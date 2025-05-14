using AutoMapper;
using Domain.Dto.Seller;
using Domain.Entities.Seller;
using Domain.Service;
using Infrastructure.Adapters.AwsS3;
using Newtonsoft.Json;
using Persistence.IRepositories.Seller;
using RestSharp;

namespace Application.Services.Seller;

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