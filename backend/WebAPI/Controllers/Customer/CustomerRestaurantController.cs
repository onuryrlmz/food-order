using Application.Services.Seller._2_RestaurantService;
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

    public CustomerRestaurantController(IRestaurantService restaurantService) => _restaurantService = restaurantService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<GetRestaurantsResponseDto>> GetList([FromQuery] GetRestaurantsRequestDto requestDto)
        => await _restaurantService.GetRestaurants(requestDto);

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<string>> GetInfo(Guid id)
        => await _restaurantService.GetRestaurantInfo(new GetRestaurantInformationRequestDto { Id = id });
}
