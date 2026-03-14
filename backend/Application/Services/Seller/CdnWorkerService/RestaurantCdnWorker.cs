using Application.Services.Common.RedisService;
using Dapper;
using Domain.Entities.Seller;
using Infrastructure.Adapters.AwsS3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.IRepositories.Seller;
using System.Data;
using System.Text;
using System.Text.Json;

namespace Application.Services.Seller.CdnWorkerService;

/// <summary>
/// CDN güncelleme kuyruğunu işleyen background worker.
/// Her 30 saniyede bir pending kayıtları kontrol eder,
/// restoran menüsünü JSON olarak üretir ve R2/CDN'e yükler.
/// </summary>
public class RestaurantCdnWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RestaurantCdnWorker> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);
    private const int MaxRetry = 3;
    private const string CdnFolder = "restaurants";

    public RestaurantCdnWorker(IServiceScopeFactory scopeFactory, ILogger<RestaurantCdnWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RestaurantCdnWorker başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingQueueAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RestaurantCdnWorker beklenmeyen hata.");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task ProcessPendingQueueAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var queueRepo = scope.ServiceProvider.GetRequiredService<IRestaurantCdnUpdateQueueRepository>();
        var s3Adapter = scope.ServiceProvider.GetRequiredService<IAwsS3ServiceAdapter>();
        var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();

        var pending = await queueRepo.GetListAsync(
            x => x.StatusId == (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Pending
                 && x.RetryCount < MaxRetry,
            orderBy: q => q.OrderBy(x => x.CreatedDate),
            size: 10,
            enableTracking: true);

        if (pending.Count == 0) return;

        _logger.LogInformation("{Count} adet CDN güncelleme işlemi işlenecek.", pending.Count);

        foreach (var item in pending.Items)
        {
            if (ct.IsCancellationRequested) break;

            item.StatusId = (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Processing;
            queueRepo.Update(item);
            await dbContext.SaveChangesAsync(ct);

            try
            {
                var json = await BuildRestaurantJsonAsync(dbContext, item.RestaurantId);
                if (string.IsNullOrEmpty(json))
                {
                    item.StatusId = (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Failed;
                    item.ErrorMessage = "JSON üretilemedi — restoran bulunamadı veya menü boş.";
                    item.RetryCount++;
                    queueRepo.Update(item);
                    await dbContext.SaveChangesAsync(ct);
                    continue;
                }

                var fileName = $"{item.RestaurantId}.json";
                var bytes = Encoding.UTF8.GetBytes(json);
                var url = await s3Adapter.UploadBytesAsync(bytes, fileName, CdnFolder, "application/json");

                if (string.IsNullOrEmpty(url))
                {
                    item.StatusId = (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Failed;
                    item.ErrorMessage = "S3/R2 yükleme başarısız.";
                    item.RetryCount++;
                    queueRepo.Update(item);
                    await dbContext.SaveChangesAsync(ct);
                    continue;
                }

                // Redis cache güncelle
                var cacheKey = $"RestaurantInfo_{item.RestaurantId}";
                await redisService.SetValueAsync(cacheKey, url, TimeSpan.FromHours(24));

                item.StatusId = (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Completed;
                item.ProcessedAt = DateTime.UtcNow;
                queueRepo.Update(item);
                await dbContext.SaveChangesAsync(ct);

                _logger.LogInformation("Restoran {RestaurantId} CDN'e yüklendi: {Url}", item.RestaurantId, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Restoran {RestaurantId} CDN güncelleme hatası.", item.RestaurantId);
                item.StatusId = (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Failed;
                item.ErrorMessage = ex.Message;
                item.RetryCount++;
                queueRepo.Update(item);
                await dbContext.SaveChangesAsync(ct);
            }
        }
    }

    /// <summary>
    /// Restoran için tam menü JSON'unu veritabanından üretir.
    /// </summary>
    private static async Task<string?> BuildRestaurantJsonAsync(BaseDbContext dbContext, Guid restaurantId)
    {
        const string query = @"
SELECT JSON_ARRAYAGG(cat.obj)
FROM (SELECT JSON_OBJECT(
                 'id', c.Id,
                 'name', c.Name,
                 'orderIndex', c.OrderIndex,
                 'categoriesDetail', (SELECT JSON_ARRAYAGG(ordered_cd.obj)
                                      FROM (SELECT JSON_OBJECT(
                                                           'id', cd.Id,
                                                           'orderIndex', cd.OrderIndex,
                                                           'menuId', cd.MenuId,
                                                           'menus', (SELECT JSON_ARRAYAGG(ordered_m.obj)
                                                                     FROM (SELECT JSON_OBJECT(
                                                                                          'id', m.Id,
                                                                                          'name', m.Name,
                                                                                          'price', m.Price,
                                                                                          'orderIndex', m.OrderIndex,
                                                                                          'menuOptions',
                                                                                          (SELECT JSON_ARRAYAGG(ordered_mo.obj)
                                                                                           FROM (SELECT JSON_OBJECT(
                                                                                                                'id', mo.Id,
                                                                                                                'name', mo.Name,
                                                                                                                'minCount', mo.MinCount,
                                                                                                                'maxCount', mo.MaxCount,
                                                                                                                'orderIndex', mo.OrderIndex,
                                                                                                                'menuOptionValues',
                                                                                                                (SELECT JSON_ARRAYAGG(ordered_mov.obj)
                                                                                                                 FROM (SELECT JSON_OBJECT(
                                                                                                                                      'id', mov.Id,
                                                                                                                                      'productId', mov.ProductId,
                                                                                                                                      'name',
                                                                                                                                      (SELECT p.Name FROM Product AS p WHERE p.Id = mov.ProductId AND p.DeletedDate IS NULL),
                                                                                                                                      'price', mov.Price,
                                                                                                                                      'orderIndex', mov.OrderIndex,
                                                                                                                                      'menuOptionValueOptions',
                                                                                                                                      (SELECT JSON_ARRAYAGG(ordered_movo.obj)
                                                                                                                                       FROM (SELECT JSON_OBJECT(
                                                                                                                                                            'id', movo.Id,
                                                                                                                                                            'name', movo.Name,
                                                                                                                                                            'minCount', movo.MinCount,
                                                                                                                                                            'maxCount', movo.MaxCount,
                                                                                                                                                            'orderIndex', movo.OrderIndex,
                                                                                                                                                            'menuOptionValueOptionValues',
                                                                                                                                                            (SELECT JSON_ARRAYAGG(ordered_movov.obj)
                                                                                                                                                             FROM (SELECT JSON_OBJECT(
                                                                                                                                                                                  'id', movov.Id,
                                                                                                                                                                                  'productId', movov.ProductId,
                                                                                                                                                                                  'name',
                                                                                                                                                                                  (SELECT p.Name FROM Product AS p WHERE p.Id = movov.ProductId AND p.DeletedDate IS NULL),
                                                                                                                                                                                  'price', movov.Price,
                                                                                                                                                                                  'orderIndex', movov.OrderIndex
                                                                                                                                                                          ) AS obj
                                                                                                                                                                   FROM MenuOptionValueOptionValue AS movov
                                                                                                                                                                   WHERE movov.MenuOptionValueOptionId = movo.Id AND movov.DeletedDate IS NULL
                                                                                                                                                                   ORDER BY movov.OrderIndex ASC) AS ordered_movov)
                                                                                                                                                    ) AS obj
                                                                                                                                             FROM MenuOptionValueOption AS movo
                                                                                                                                             WHERE movo.MenuOptionValueId = mov.Id AND movo.DeletedDate IS NULL
                                                                                                                                             ORDER BY movo.OrderIndex ASC) AS ordered_movo)
                                                                                                                              ) AS obj
                                                                                                                       FROM MenuOptionValue AS mov
                                                                                                                       WHERE mov.MenuOptionId = mo.Id AND mov.DeletedDate IS NULL
                                                                                                                       ORDER BY mov.OrderIndex ASC) AS ordered_mov)
                                                                                                        ) AS obj
                                                                                                 FROM MenuOption AS mo
                                                                                                 WHERE mo.MenuId = m.Id AND mo.DeletedDate IS NULL
                                                                                                 ORDER BY mo.OrderIndex ASC) AS ordered_mo)
                                                                                  ) AS obj
                                                                           FROM Menu AS m
                                                                           WHERE m.Id = cd.MenuId AND m.DeletedDate IS NULL
                                                                           ORDER BY m.OrderIndex ASC) AS ordered_m)
                                                   ) AS obj
                                            FROM CategoryDetail AS cd
                                            WHERE cd.CategoryId = c.Id AND cd.DeletedDate IS NULL
                                            ORDER BY cd.OrderIndex ASC) AS ordered_cd)
         ) AS obj
  FROM Category AS c
  WHERE c.RestaurantId = @restaurantId AND c.DeletedDate IS NULL
  ORDER BY c.OrderIndex ASC) AS cat;
";

        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        var jsonData = await conn.QueryFirstOrDefaultAsync<string>(query, new { restaurantId = restaurantId.ToString() });
        return jsonData;
    }
}
