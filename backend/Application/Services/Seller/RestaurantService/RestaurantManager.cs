using System.Data;
using Application.Services.Common.RedisService;
using Application.Services.Common.TokenService;
using AutoMapper;
using Dapper;
using Domain.Dto.Seller.Restaurant;
using Domain.Entities.Seller;
using Domain.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence.Contexts;
using Persistence.IRepositories.Common;
using Persistence.IRepositories.Seller;
using Base.Enums;

namespace Application.Services.Seller.RestaurantService;

public class RestaurantManager : IRestaurantService
{
    private const double GridSizeDegree = 0.00225;
    private readonly IMapper _mapper;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly BaseDbContext _context;
    private readonly IRedisService _redisService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantCdnUpdateQueueRepository _cdnQueueRepository;
    private readonly IRestaurantWorkingHourRepository _workingHourRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IUserRepository _userRepository;

    public RestaurantManager(BaseDbContext context, IRestaurantRepository restaurantRepository, IMapper mapper, IRedisService redisService, IAddressRepository addressRepository, ISellerRepository sellerRepository, IUserRepository userRepository, ITokenAccessor tokenAccessor, IRestaurantCdnUpdateQueueRepository cdnQueueRepository, IRestaurantWorkingHourRepository workingHourRepository)
    {
        _context = context;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _redisService = redisService;
        _addressRepository = addressRepository;
        _sellerRepository = sellerRepository;
        _userRepository = userRepository;
        _tokenAccessor = tokenAccessor;
        _cdnQueueRepository = cdnQueueRepository;
        _workingHourRepository = workingHourRepository;
    }

    public async Task<ServiceObjectResult<Guid>> AddRestaurant(AddRestaurantDto requestDto)
    {
        var result = new ServiceObjectResult<Guid>();
        try
        {
            var restaurant = _mapper.Map<Restaurant>(requestDto);
            restaurant.Id = Guid.NewGuid();
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
            double lat, lng;

            // Doğrudan lat/lng verilmişse kullan (login olmadan konum bazlı arama)
            if (requestRequestDto.Latitude.HasValue && requestRequestDto.Longitude.HasValue)
            {
                lat = requestRequestDto.Latitude.Value;
                lng = requestRequestDto.Longitude.Value;
            }
            else
            {
                var address = await _addressRepository.GetAsync(x => x.Id == requestRequestDto.AddressId);
                if (address == null)
                {
                    result.Fail("Address not found");
                    return result;
                }

                if (string.IsNullOrEmpty(address.Latitude) || string.IsNullOrEmpty(address.Longitude))
                {
                    result.SetData(new List<GetRestaurantsResponseDto>());
                    return result;
                }

                lat = double.Parse(address.Latitude, System.Globalization.CultureInfo.InvariantCulture);
                lng = double.Parse(address.Longitude, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (lat < 36.0 || lat > 42.0 || lng < 26.0 || lng > 45.0)
            {
                result.SetData(new List<GetRestaurantsResponseDto>());
                return result;
            }

            var hasFilters = !string.IsNullOrWhiteSpace(requestRequestDto.Search)
                             || requestRequestDto.CuisineId.HasValue
                             || requestRequestDto.MinRating.HasValue
                             || requestRequestDto.MaxMinOrder.HasValue
                             || requestRequestDto.IsOpen.HasValue
                             || requestRequestDto.SortBy != null
                             || requestRequestDto.Page > 1;

            var restaurantList = new List<GetRestaurantsResponseDto>();

            if (!hasFilters)
            {
                var gridId = GetGridId(lat, lng);
                var cachedJson = await _redisService.GetValueAsync<string>(gridId);

                if (!string.IsNullOrEmpty(cachedJson))
                {
                    restaurantList = JsonConvert.DeserializeObject<List<GetRestaurantsResponseDto>>(cachedJson);
                }
                else
                {
                    var resultPolygon = await GetRestaurantsByPolygon(lat, lng, requestRequestDto);
                    if (resultPolygon.HasFailed)
                    {
                        result.Fail(resultPolygon.Messages);
                        return result;
                    }

                    restaurantList = resultPolygon.Data;
                    await _redisService.SetValueAsync(gridId, JsonConvert.SerializeObject(restaurantList), TimeSpan.FromMinutes(15));
                }
            }
            else
            {
                var resultPolygon = await GetRestaurantsByPolygon(lat, lng, requestRequestDto);
                if (resultPolygon.HasFailed)
                {
                    result.Fail(resultPolygon.Messages);
                    return result;
                }

                restaurantList = resultPolygon.Data;
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

    private async Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetRestaurantsByPolygon(double lat, double lng, GetRestaurantsRequestDto? filters = null)
    {
        var result = new ServiceCollectionResult<GetRestaurantsResponseDto>();
        try
        {
            var whereClauses = new List<string>
            {
                "r.`ServiceAreaPolygonWkt` IS NOT NULL",
                "ST_Contains(ST_GeomFromText(r.`ServiceAreaPolygonWkt`), ST_GeomFromText(CONCAT('POINT(', @lng, ' ', @lat, ')')))",
                "r.`IsActive` = 1",
                "r.`DeletedDate` IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("lat", lat);
            parameters.Add("lng", lng);

            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.Search))
                {
                    whereClauses.Add("r.`Name` LIKE @search");
                    parameters.Add("search", $"%{filters.Search}%");
                }

                if (filters.MinRating.HasValue)
                {
                    whereClauses.Add("r.`Rating` >= @minRating");
                    parameters.Add("minRating", filters.MinRating.Value);
                }

                if (filters.MaxMinOrder.HasValue)
                {
                    whereClauses.Add("r.`MinimumOrderPrice` <= @maxMinOrder");
                    parameters.Add("maxMinOrder", filters.MaxMinOrder.Value);
                }

                if (filters.IsOpen.HasValue)
                {
                    whereClauses.Add("r.`IsOpen` = @isOpen");
                    parameters.Add("isOpen", filters.IsOpen.Value);
                }
            }

            var orderBy = "r.`Rating` DESC";
            if (filters?.SortBy != null)
                orderBy = filters.SortBy switch
                {
                    "rating" => "r.`Rating` DESC",
                    "minOrder" => "r.`MinimumOrderPrice` ASC",
                    "deliveryTime" => "r.`MinDeliveryTime` ASC",
                    "name" => "r.`Name` ASC",
                    _ => "r.`Rating` DESC"
                };

            var page = filters?.Page ?? 1;
            var pageSize = Math.Min(filters?.PageSize ?? 50, 100);
            var offset = (page - 1) * pageSize;
            parameters.Add("pageSize", pageSize);
            parameters.Add("offset", offset);

            var query = $@"
        SELECT
            r.`Id`,
            r.`SellerId`,
            r.`Name`,
            r.`Description`,
            r.`CoverImage` AS ImageUrl,
            r.`MinimumOrderPrice` AS MinBasketPrice,
            0 AS DeliveryPrice,
            r.`MinDeliveryTime`,
            r.`MaxDeliveryTime`
        FROM `Restaurant` r
        WHERE {string.Join(" AND ", whereClauses)}
        ORDER BY {orderBy}
        LIMIT @pageSize OFFSET @offset
    ";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var restaurants = (await conn.QueryAsync<GetRestaurantsResponseDto>(query, parameters)).ToList();

            if (restaurants.Count > 0)
            {
                var categoryQuery = @"
        SELECT c.`RestaurantId`, c.`Name`
        FROM `Category` c
        WHERE c.`RestaurantId` IN (@RestaurantIds) AND c.`DeletedDate` IS NULL
        ORDER BY c.`OrderIndex`
    ";

                var ids = restaurants.Select(x => x.Id.ToString()).ToList();
                categoryQuery = categoryQuery.Replace("@RestaurantIds", string.Join(", ", ids.Select(id => $"'{id}'")));
                var categories = await conn.QueryAsync<(Guid RestaurantId, string Name)>(categoryQuery);
                var catMap = categories.GroupBy(c => c.RestaurantId)
                    .ToDictionary(g => g.Key, g => g.Select(c => c.Name).ToList());
                foreach (var r in restaurants)
                    if (catMap.TryGetValue(r.Id, out var cats))
                        r.Categories = cats;
            }
            else
            {
                result.SetData(new List<GetRestaurantsResponseDto>());
                return result;
            }

            result.SetData(restaurants);
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
            var cdnUrl = await _redisService.GetValueAsync<string>(cacheKey);

            if (!string.IsNullOrEmpty(cdnUrl))
            {
                result.SetData(cdnUrl);
                return result;
            }

            // CDN URL henüz hazır değil — worker kuyruğuna ekle
            var alreadyQueued = await _cdnQueueRepository.GetAsync(x => x.RestaurantId == requestDto.Id
                                                                        && x.StatusId == (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Pending);

            if (alreadyQueued == null)
                await _cdnQueueRepository.AddAsync(new RestaurantCdnUpdateQueue
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = requestDto.Id
                });

            result.Fail("Restaurant menu is being prepared. Please try again shortly.");
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetRestaurantListForSellerResponseDto>> GetRestaurantListForSeller()
    {
        var result = new ServiceCollectionResult<GetRestaurantListForSellerResponseDto>();
        try
        {
            var user = await _userRepository.GetAsync(x => x.Id == _tokenAccessor.GetToken().UserId);
            if (user == null)
            {
                result.Fail("User not found");
                return result;
            }

            var seller = await _sellerRepository.GetAsync(x => x.Id == user.SellerId);
            if (seller == null)
            {
                result.Fail("Seller not found");
                return result;
            }

            var restaurants = await _restaurantRepository.GetListAsync(x => x.SellerId == user.SellerId, enableTracking: false, size: 999);
            if (restaurants == null || restaurants.Count == 0)
            {
                result.Fail("Restaurant not found");
                return result;
            }

            var restaurantList = restaurants.Items.Select(x => new GetRestaurantListForSellerResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                IsActive = x.IsActive,
                IsOpen = x.IsOpen,
                MinimumOrderPrice = x.MinimumOrderPrice,
                MinDeliveryTime = x.MinDeliveryTime,
                MaxDeliveryTime = x.MaxDeliveryTime,
                Rating = x.Rating,
                RatingCount = x.RatingCount,
                CoverImage = x.CoverImage,
                Description = x.Description
            }).ToList();

            result.SetData(restaurantList);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateRestaurant(UpdateRestaurantDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var restaurant = await _restaurantRepository.GetAsync(
                x => x.Id == requestDto.Id && x.SellerId == token!.SellerId, enableTracking: true);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            restaurant.Name = requestDto.Name;
            restaurant.Phone = requestDto.Phone;
            restaurant.Email = requestDto.Email;
            restaurant.MinimumOrderPrice = requestDto.MinimumOrderPrice;
            restaurant.MinDeliveryTime = requestDto.MinDeliveryTime;
            restaurant.MaxDeliveryTime = requestDto.MaxDeliveryTime;
            restaurant.Description = requestDto.Description;
            restaurant.CoverImage = requestDto.CoverImage;

            await _restaurantRepository.UpdateAsync(restaurant);

            var alreadyQueued = await _cdnQueueRepository.GetAsync(x => x.RestaurantId == requestDto.Id && x.StatusId == (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Pending);
            if (alreadyQueued == null)
                await _cdnQueueRepository.AddAsync(new RestaurantCdnUpdateQueue { Id = Guid.NewGuid(), RestaurantId = requestDto.Id });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ToggleOpen(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var restaurant = await _restaurantRepository.GetAsync(
                x => x.Id == restaurantId && x.SellerId == token!.SellerId, enableTracking: true);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            restaurant.IsOpen = !restaurant.IsOpen;
            await _restaurantRepository.UpdateAsync(restaurant);
            result.SetData(restaurant.IsOpen);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ToggleActive(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == restaurantId, enableTracking: true);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            restaurant.IsActive = !restaurant.IsActive;
            await _restaurantRepository.UpdateAsync(restaurant);
            result.SetData(restaurant.IsActive);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> ApproveRestaurant(Guid restaurantId)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == restaurantId, enableTracking: true);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            if (restaurant.ApprovedAt != null)
            {
                result.Fail("Restoran zaten onaylanmış.");
                return result;
            }

            var token = _tokenAccessor.GetToken();

            restaurant.IsActive = true;
            restaurant.ApprovedAt = DateTime.UtcNow;
            restaurant.ApprovedByUserId = token?.UserId;
            await _restaurantRepository.UpdateAsync(restaurant);

            // Komisyon: restorana özel bir kayıt oluşturulmaz. ResolveCommission, özel kayıt yokken
            // platform varsayılanını (PlatformCommissionSchedule) uygular; böylece platform oranı
            // değiştiğinde tüm onaylı restoranlara otomatik yansır. Özel oran gerektiğinde admin
            // SetRestaurantCommission ile açıkça tanımlar.
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<string>> GetRestaurantInfoForSeller(GetRestaurantInformationRequestDto requestDto)
    {
        var result = new ServiceObjectResult<string>();
        try
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
      WHERE c.RestaurantId = @restaurantId
        AND c.DeletedDate IS NULL
      ORDER BY c.OrderIndex ASC) AS cat;
";

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            var jsonData = await conn.QueryFirstOrDefaultAsync<string>(query, new { restaurantId = requestDto.Id.ToString() });

            if (string.IsNullOrEmpty(jsonData))
            {
                result.Fail("Restaurant not found");
                return result;
            }

            result.SetData(jsonData);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<GetAdminRestaurantListResponseDto>> GetAllRestaurantsForAdmin(int page = 1, int pageSize = 50)
    {
        var result = new ServiceCollectionResult<GetAdminRestaurantListResponseDto>();
        try
        {
            pageSize = Math.Min(pageSize, 200);
            var restaurants = await _restaurantRepository.GetListAsync(
                null,
                q => q.OrderByDescending(r => r.CreatedDate),
                index: page - 1,
                size: pageSize);

            var dtos = restaurants.Items.Select(r => new GetAdminRestaurantListResponseDto
            {
                Id = r.Id,
                SellerId = r.SellerId,
                Name = r.Name,
                Phone = r.Phone,
                Email = r.Email,
                Description = r.Description,
                IsActive = r.IsActive,
                IsOpen = r.IsOpen,
                Rating = r.Rating,
                RatingCount = r.RatingCount,
                MinimumOrderPrice = r.MinimumOrderPrice,
                MinDeliveryTime = r.MinDeliveryTime,
                MaxDeliveryTime = r.MaxDeliveryTime,
                CoverImage = r.CoverImage,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                ServiceAreaPolygonWkt = r.ServiceAreaPolygonWkt,
                CreatedDate = r.CreatedDate
            }).ToList();

            result.SetData(restaurants.Count, dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<GetAdminRestaurantListResponseDto>> GetRestaurantByIdForAdmin(Guid id)
    {
        var result = new ServiceObjectResult<GetAdminRestaurantListResponseDto>();
        try
        {
            var r = await _restaurantRepository.GetAsync(x => x.Id == id);
            if (r == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            result.SetData(new GetAdminRestaurantListResponseDto
            {
                Id = r.Id,
                SellerId = r.SellerId,
                Name = r.Name,
                Phone = r.Phone,
                Email = r.Email,
                Description = r.Description,
                IsActive = r.IsActive,
                IsOpen = r.IsOpen,
                Rating = r.Rating,
                RatingCount = r.RatingCount,
                MinimumOrderPrice = r.MinimumOrderPrice,
                MinDeliveryTime = r.MinDeliveryTime,
                MaxDeliveryTime = r.MaxDeliveryTime,
                CoverImage = r.CoverImage,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                ServiceAreaPolygonWkt = r.ServiceAreaPolygonWkt,
                CreatedDate = r.CreatedDate
            });
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpdateRestaurantForAdmin(UpdateAdminRestaurantDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.Id, enableTracking: true);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            restaurant.Name = requestDto.Name;
            restaurant.Phone = requestDto.Phone;
            restaurant.Email = requestDto.Email;
            restaurant.MinimumOrderPrice = requestDto.MinimumOrderPrice;
            restaurant.MinDeliveryTime = requestDto.MinDeliveryTime;
            restaurant.MaxDeliveryTime = requestDto.MaxDeliveryTime;
            restaurant.Description = requestDto.Description;
            restaurant.Latitude = requestDto.Latitude;
            restaurant.Longitude = requestDto.Longitude;
            restaurant.ServiceAreaPolygonWkt = requestDto.ServiceAreaPolygonWkt;

            await _restaurantRepository.UpdateAsync(restaurant);

            // CDN kuyruğuna ekle (menü bilgisi değişti)
            var alreadyQueued = await _cdnQueueRepository.GetAsync(x => x.RestaurantId == requestDto.Id && x.StatusId == (short)RestaurantCdnUpdateQueue.CdnUpdateStatus.Pending);
            if (alreadyQueued == null)
                await _cdnQueueRepository.AddAsync(new RestaurantCdnUpdateQueue { Id = Guid.NewGuid(), RestaurantId = requestDto.Id });

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceCollectionResult<WorkingHourDto>> GetWorkingHours(Guid restaurantId)
    {
        var result = new ServiceCollectionResult<WorkingHourDto>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == restaurantId && x.SellerId == token!.SellerId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            var hours = await _workingHourRepository.GetListAsync(x => x.RestaurantId == restaurantId, size: 7);
            var dtos = hours.Items.Select(h => new WorkingHourDto
            {
                Id = h.Id,
                RestaurantId = h.RestaurantId,
                DayOfWeek = h.DayOfWeek,
                OpenTime = h.OpenTime,
                CloseTime = h.CloseTime,
                IsClosed = h.IsClosed
            }).ToList();
            result.SetData(dtos);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<bool>> UpsertWorkingHour(UpsertWorkingHourDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var token = _tokenAccessor.GetToken();
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId && x.SellerId == token!.SellerId);
            if (restaurant == null)
            {
                result.Fail("Restoran bulunamadı.");
                return result;
            }

            var existing = await _workingHourRepository.GetAsync(x => x.RestaurantId == requestDto.RestaurantId && x.DayOfWeek == requestDto.DayOfWeek, enableTracking: true);
            if (existing != null)
            {
                existing.OpenTime = requestDto.OpenTime;
                existing.CloseTime = requestDto.CloseTime;
                existing.IsClosed = requestDto.IsClosed;
                await _workingHourRepository.UpdateAsync(existing);
            }
            else
            {
                await _workingHourRepository.AddAsync(new RestaurantWorkingHour
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = requestDto.RestaurantId,
                    DayOfWeek = requestDto.DayOfWeek,
                    OpenTime = requestDto.OpenTime,
                    CloseTime = requestDto.CloseTime,
                    IsClosed = requestDto.IsClosed
                });
            }

            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}