using Application.Services.Seller.RestaurantService;
using Base.Enums;
using Domain.Dto.Seller.Restaurant;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/restaurant")]
[ApiController]
public class CustomerRestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;

    public CustomerRestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(false, false)]
    public async Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetList([FromQuery] GetRestaurantsRequestDto requestDto)
    {
        return await _restaurantService.GetRestaurants(requestDto);
    }

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(false, false)]
    public async Task<ServiceObjectResult<string>> GetInfo(Guid id)
    {
        return await _restaurantService.GetRestaurantInfo(new GetRestaurantInformationRequestDto { Id = id });
    }
}