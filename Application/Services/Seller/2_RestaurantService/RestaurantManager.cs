using AutoMapper;
using Domain.Dto.Seller;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.AwsS3;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;
using RestSharp;

namespace Application.Services.Seller._2_RestaurantService;

public class RestaurantManager : IRestaurantService
{
    private readonly IMapper _mapper;
    private readonly IRestaurantRepository _restaurantRepository;

    public RestaurantManager(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
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

    public async Task<ServiceObjectResult<bool>> CreateRestaurantInformationJsonFile(CreateRestaurantInformationJsonFileRequestDto requestDto)
    {
        var result = new ServiceObjectResult<bool>();
        try
        {
            var restaurant = await _restaurantRepository.GetAsync(x => x.Id == requestDto.RestaurantId);
            if (restaurant == null)
            {
                result.Fail("Restaurant not found");
                return result;
            }

            var mdl = new RestaurantResponseDto();
            // var responseCategories = await _categoryService.GetCategoriesByRestaurantId(null, new GetCategoriesByRestaurantIdQuery
            // {
            //     RestaurantId = restaurant.Id,
            //     GetMenus = true,
            //     GetMenuProducts = false
            // });
            // mdl.Categories = responseCategories.Data;

            var json = JsonConvert.SerializeObject(mdl);
            var pathFolder = Path.Combine("wwwroot", "restaurants");
            var pathFile = Path.Combine(pathFolder, $"{restaurant.Id}.json");

            if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);
            if (File.Exists(pathFile)) File.Delete(pathFile);
            await File.WriteAllTextAsync(pathFile, json);

            var _awsS3ServiceAdapter = new AwsS3ServiceAdapter();
            var url = _awsS3ServiceAdapter.UploadFileAsync(pathFile, @"restaurant");
            if (url == null)
            {
                result.Fail("File not uploaded");
                return result;
            }

            if (File.Exists(pathFile)) File.Delete(pathFile);
            result.SetData(true);
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }

    public async Task<ServiceObjectResult<RestaurantResponseDto>> GetRestaurantInformationByRestaurantId(GetRestaurantInformationRequestDto requestDto)
    {
        var result = new ServiceObjectResult<RestaurantResponseDto>();
        try
        {
            var options = new RestClientOptions("https://cdn.yrlmzteknoloji.com")
            {
                MaxTimeout = -1
            };
            var restClient = new RestClient(options);
            var restRequest = new RestRequest($@"/restaurant/{requestDto.Id}.json");
            var response = await restClient.ExecuteAsync(restRequest, CancellationToken.None);
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var restaurant = JsonConvert.DeserializeObject<RestaurantResponseDto>(response.Content);
                result.SetData(restaurant);
            }
            else
            {
                result.Fail("Restaurant not found.");
            }
        }
        catch (Exception e)
        {
            result.Fail(e);
        }

        return result;
    }
}

/*
SELECT dm.target_menu_id, -- Bu JSON'ın üretildiği MenuId
          (SELECT JSON_ARRAYAGG(top_level_cat.obj)
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
                                                                                          ORDER BY m.OrderIndex
                                                                                              ASC) AS ordered_m)
                                                                  ) AS obj
                                                           FROM CategoryDetail AS cd
                                                           WHERE cd.CategoryId = c.Id
                                                             AND cd.DeletedDate IS NULL
                                                             AND cd.MenuId = dm.target_menu_id
                                                           ORDER BY cd.OrderIndex ASC) AS ordered_cd)
                        ) AS obj
                 FROM Category AS c
                 WHERE c.RestaurantId = '75dedf44-69c1-4896-805e-eac5bd87e6d0'
                   AND c.DeletedDate IS NULL
                   AND EXISTS (SELECT 1
                               FROM CategoryDetail cd_exists
                               WHERE cd_exists.CategoryId = c.Id
                                 AND cd_exists.MenuId = dm.target_menu_id
                                 AND cd_exists.DeletedDate IS NULL)
                 ORDER BY c.OrderIndex ASC) AS top_level_cat) AS menu_json_data
   FROM (SELECT DISTINCT cd_outer.MenuId AS target_menu_id
         FROM CategoryDetail cd_outer
                  JOIN Category c_outer ON c_outer.Id = cd_outer.CategoryId
         WHERE c_outer.RestaurantId = '75dedf44-69c1-4896-805e-eac5bd87e6d0'
           AND cd_outer.DeletedDate IS NULL
           AND c_outer.DeletedDate IS NULL
           AND cd_outer.MenuId IS NOT NULL) AS dm
   ORDER BY dm.target_menu_id;

*/

/*
select c.Id                                                                                     as cId,
          c.Name                                                                                   as cName,
          c.OrderIndex                                                                             as cOrderIndex,

          cd.Id                                                                                    as cdId,
          cd.OrderIndex                                                                            as cdOrderIndex,
          cd.MenuId                                                                                as cdMenuId,

          m.Id                                                                                     as mId,
          m.Name                                                                                   as mName,
          m.Price                                                                                  as mPrice,
          m.OrderIndex                                                                             as mOrderIndex,

          mo.Id                                                                                    as moId,
          mo.Name                                                                                  as moName,
          mo.MinCount                                                                              as moMinCount,
          mo.MaxCount                                                                              as moMaxCount,
          mo.OrderIndex                                                                            as moOrderIndex,

          mov.Id                                                                                   as movId,
          mov.ProductId                                                                            as movProductId,
          (select p.Name from Product as p where p.Id = mov.ProductId and p.DeletedDate is null)   as movName,
          mov.Price                                                                                as movPrice,
          mov.OrderIndex                                                                           as movOrderIndex,

          movo.Id                                                                                  as movoId,
          movo.Name                                                                                as movoName,
          movo.MinCount                                                                            as movoMinCount,
          movo.MaxCount                                                                            as movoMaxCount,
          movo.OrderIndex                                                                          as movoOrderIndex,

          movov.Id                                                                                 as movovId,
          movov.ProductId                                                                          as movovProductId,
          (select p.Name from Product as p where p.Id = movov.ProductId and p.DeletedDate is null) as movovName,
          movov.Price                                                                              as movovPrice,
          movov.OrderIndex                                                                         as movovOrderIndex
   from Category as c
            left join CategoryDetail as cd on cd.CategoryId = c.Id and cd.DeletedDate is null
            left join Menu as m on m.Id = cd.MenuId and m.DeletedDate is null
            left join MenuOption as mo on mo.MenuId = m.Id and mo.DeletedDate is null
            left join MenuOptionValue as mov on mov.MenuOptionId = mo.Id and mov.DeletedDate is null
            left join MenuOptionValueOption as movo on movo.MenuOptionValueId = mov.Id and movo.DeletedDate is null
            left join MenuOptionValueOptionValue as movov
                      on movov.MenuOptionValueOptionId = movo.Id and movov.DeletedDate is null
   where c.RestaurantId = '75dedf44-69c1-4896-805e-eac5bd87e6d0'
     and c.DeletedDate is null
     and cd.MenuId = '7280cfbb-fa34-45e7-a44d-897e86b144b6'
   order by moOrderIndex;
*/