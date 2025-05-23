using System.Data;
using Application.Services.Common;
using AutoMapper;
using Dapper;
using Domain.Dto.Seller.Restaurant;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.AwsS3;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence.Contexts;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;

namespace Application.Services.Seller._2_RestaurantService;

public class RestaurantManager : IRestaurantService
{
    private const double GridSizeDegree = 0.005;
    private readonly IMapper _mapper;
    private readonly BaseDbContext _context;
    private readonly IRedisService _redisService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IAddressRepository _addressRepository;

    public RestaurantManager(BaseDbContext context, IRestaurantRepository restaurantRepository, IMapper mapper, IRedisService redisService, IAddressRepository addressRepository)
    {
        _context = context;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _redisService = redisService;
        _addressRepository = addressRepository;
    }

    public async Task<ServiceObjectResult<Guid>> AddRestaurant(AddRestaurantDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var restaurant = _mapper.Map<Restaurant>(requestDto);
            restaurant.Id = Guid.NewGuid();
            restaurant.OrderIndex = requestDto.OrderIndex;
            await _restaurantRepository.AddAsync(restaurant);
            result.SetData(restaurant.Id);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    #region Get Restaurant List

    public async Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetRestaurants(GetRestaurantsRequestDto requestRequestDto)
    {
        var result = new ServiceCollectionResult<GetRestaurantsResponseDto>();
        try
        {
            var address = await _addressRepository.GetAsync(x => x.Id == requestRequestDto.AddressId);
            if (address == null)
            {
                result.Fail("Address not found");
                return result;
            }

            var gridId = GetGridId(Convert.ToDouble(address.Latitude), Convert.ToDouble(address.Longitude));

            var cachedJson = await _redisService.GetValueAsync<string>(gridId);
            var restaurantList = new List<GetRestaurantsResponseDto>();

            if (!string.IsNullOrEmpty(cachedJson))
            {
                restaurantList = JsonConvert.DeserializeObject<List<GetRestaurantsResponseDto>>(cachedJson);
            }
            else
            {
                var resultPolygon = await GetRestaurantsByPolygon(Convert.ToDouble(address.Latitude), Convert.ToDouble(address.Longitude));
                if (resultPolygon.HasFailed)
                {
                    result.Fail(resultPolygon.Messages);
                    return result;
                }

                restaurantList = resultPolygon.Data;
                await _redisService.SetValueAsync(gridId, JsonConvert.SerializeObject(restaurantList), TimeSpan.FromMinutes(15));
            }

            result.SetData(restaurantList);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    private static string GetGridId(double latitude, double longitude)
    {
        if (latitude < 36.0 || latitude > 42.0 || longitude < 26.0 || longitude > 45.0)
            throw new Exception("Türkiye sınırları dışında!");

        var latIndex = (int)((latitude - 36.0) / GridSizeDegree);
        var lngIndex = (int)((longitude - 26.0) / GridSizeDegree);
        return $"grid_{latIndex}_{lngIndex}";
    }

    private async Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetRestaurantsByPolygon(double lat, double lng)
    {
        var result = new ServiceCollectionResult<GetRestaurantsResponseDto>();
        try
        {
            const string query = @"
        SELECT 
            id, 
            name, 
            description,
            cover_photo as CoverPhoto,
            min_basket as MinBasket,
            estimated_delivery_time as DeliveryTime,
            categories,
            is_open as IsOpen,
            ST_Distance_Sphere(servis_alani_polygon, POINT(@lng, @lat)) as Distance
        FROM restaurants
        WHERE 
            ST_Contains(servis_alani_polygon, POINT(@lng, @lat))
            AND is_active = 1
    ";

            await using var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var data = await conn.QueryAsync<GetRestaurantsResponseDto>(
                query,
                new { lat, lng }
            );

            result.SetData(data.ToList());
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    #endregion

    public async Task<ServiceObjectResult<string>> GetRestaurantInfo(GetRestaurantInformationRequestDto requestDto)
    {
        var result = new ServiceObjectResult<string>();
        try
        {
            var cacheKey = $"RestaurantInfo_{requestDto.Id}";
            var cachedData = await _redisService.GetValueAsync<string>(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                result.SetData(cachedData);
                return result;
            }

            var query = $@"
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
                                                                                              'orderIndex',
                                                                                              m.OrderIndex,
                                                                                              'menuOptions',
                                                                                              (SELECT JSON_ARRAYAGG(ordered_mo.obj)
                                                                                               FROM (SELECT JSON_OBJECT(
                                                                                                                    'id',
                                                                                                                    mo.Id,
                                                                                                                    'name',
                                                                                                                    mo.Name,
                                                                                                                    'minCount',
                                                                                                                    mo.MinCount,
                                                                                                                    'maxCount',
                                                                                                                    mo.MaxCount,
                                                                                                                    'orderIndex',
                                                                                                                    mo.OrderIndex,
                                                                                                                    'menuOptionValues',
                                                                                                                    (SELECT JSON_ARRAYAGG(ordered_mov.obj)
                                                                                                                     FROM (SELECT JSON_OBJECT(
                                                                                                                                          'id',
                                                                                                                                          mov.Id,
                                                                                                                                          'productId',
                                                                                                                                          mov.ProductId,
                                                                                                                                          'name',
                                                                                                                                          (SELECT p.Name
                                                                                                                                           FROM Product AS p
                                                                                                                                           WHERE p.Id = mov.ProductId
                                                                                                                                             AND p.DeletedDate IS NULL),
                                                                                                                                          'price',
                                                                                                                                          mov.Price,
                                                                                                                                          'orderIndex',
                                                                                                                                          mov.OrderIndex,
                                                                                                                                          'menuOptionValueOptions',
                                                                                                                                          (SELECT JSON_ARRAYAGG(ordered_movo.obj)
                                                                                                                                           FROM (SELECT JSON_OBJECT(
                                                                                                                                                                'id',
                                                                                                                                                                movo.Id,
                                                                                                                                                                'name',
                                                                                                                                                                movo.Name,
                                                                                                                                                                'minCount',
                                                                                                                                                                movo.MinCount,
                                                                                                                                                                'maxCount',
                                                                                                                                                                movo.MaxCount,
                                                                                                                                                                'orderIndex',
                                                                                                                                                                movo.OrderIndex,
                                                                                                                                                                'menuOptionValueOptionValues',
                                                                                                                                                                (SELECT JSON_ARRAYAGG(ordered_movov.obj)
                                                                                                                                                                 FROM (SELECT JSON_OBJECT(
                                                                                                                                                                                      'id',
                                                                                                                                                                                      movov.Id,
                                                                                                                                                                                      'productId',
                                                                                                                                                                                      movov.ProductId,
                                                                                                                                                                                      'name',
                                                                                                                                                                                      (SELECT p.Name
                                                                                                                                                                                       FROM Product AS p
                                                                                                                                                                                       WHERE p.Id = movov.ProductId
                                                                                                                                                                                         AND p.DeletedDate IS NULL),
                                                                                                                                                                                      'price',
                                                                                                                                                                                      movov.Price,
                                                                                                                                                                                      'orderIndex',
                                                                                                                                                                                      movov.OrderIndex
                                                                                                                                                                              ) AS obj
                                                                                                                                                                       FROM MenuOptionValueOptionValue AS movov
                                                                                                                                                                       WHERE movov.MenuOptionValueOptionId = movo.Id
                                                                                                                                                                         AND movov.DeletedDate IS NULL
                                                                                                                                                                       ORDER BY movov.OrderIndex
                                                                                                                                                                           ASC) AS ordered_movov)
                                                                                                                                                        ) AS obj
                                                                                                                                                 FROM MenuOptionValueOption AS movo
                                                                                                                                                 WHERE movo.MenuOptionValueId = mov.Id
                                                                                                                                                   AND movo.DeletedDate IS NULL
                                                                                                                                                 ORDER BY movo.OrderIndex
                                                                                                                                                     ASC) AS ordered_movo)
                                                                                                                                  ) AS obj
                                                                                                                           FROM MenuOptionValue AS mov
                                                                                                                           WHERE mov.MenuOptionId = mo.Id
                                                                                                                             AND mov.DeletedDate IS NULL
                                                                                                                           ORDER BY mov.OrderIndex
                                                                                                                               ASC) AS ordered_mov)
                                                                                                            ) AS obj
                                                                                                     FROM MenuOption AS mo
                                                                                                     WHERE mo.MenuId = m.Id
                                                                                                       AND mo.DeletedDate IS NULL
                                                                                                     ORDER BY mo.OrderIndex
                                                                                                         ASC) AS ordered_mo)
                                                                                      ) AS obj
                                                                               FROM Menu AS m
                                                                               WHERE m.Id = cd.MenuId
                                                                                 AND m.DeletedDate IS NULL
                                                                               ORDER BY m.OrderIndex ASC) AS ordered_m)
                                                       ) AS obj
                                                FROM CategoryDetail AS cd
                                                WHERE cd.CategoryId = c.Id
                                                  AND cd.DeletedDate IS NULL
                                                ORDER BY cd.OrderIndex ASC) AS ordered_cd)
             ) AS obj
      FROM Category AS c
      WHERE c.RestaurantId = '{requestDto.Id}'
        AND c.DeletedDate IS NULL
      ORDER BY c.OrderIndex ASC) AS cat;
";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            var jsonData = await conn.QueryFirstAsync<string>(query);

            if (string.IsNullOrEmpty(jsonData))
            {
                result.Fail("Restaurant not found");
                return result;
            }

            var pathFolder = Path.Combine("wwwroot", "restaurants");
            if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);

            var jsonFilePath = Path.Combine(pathFolder, $"{requestDto.Id}.json");
            await File.WriteAllTextAsync(jsonFilePath, jsonData);

            var zipFilePath = Path.Combine(pathFolder, $"{requestDto.Id}.zip");
            var zipPassword = requestDto.Id.ToString("N").Substring(requestDto.Id.ToString("N").Length - 6);

            using (var zip = new ZipFile())
            {
                zip.Password = zipPassword;
                zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                zip.AddFile(jsonFilePath, "");
                zip.Save(zipFilePath);
            }

            var _awsS3ServiceAdapter = new AwsS3ServiceAdapter();
            var url = _awsS3ServiceAdapter.UploadFileAsync(zipFilePath, "restaurant");

            if (string.IsNullOrEmpty(url))
            {
                result.Fail("File not uploaded");
                return result;
            }

            if (File.Exists(jsonFilePath)) File.Delete(jsonFilePath);
            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);

            await _redisService.SetValueAsync(cacheKey, url, TimeSpan.FromMinutes(30));

            result.SetData(url);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}