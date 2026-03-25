using Application.Services.Seller.RestaurantService;
using Base.Enums;
using Domain.Dto.Seller.Restaurant;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/restaurant")]
[ApiController]
public class SellerRestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;

    public SellerRestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetRestaurantListForSellerResponseDto>> GetList()
    {
        return await _restaurantService.GetRestaurantListForSeller();
    }

    [HttpPost("add")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddRestaurantDto requestDto)
    {
        return await _restaurantService.AddRestaurant(requestDto);
    }

    [HttpPut("update")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateRestaurantDto requestDto)
    {
        return await _restaurantService.UpdateRestaurant(requestDto);
    }

    [HttpGet("{id}/info")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<string>> GetInfo(Guid id)
    {
        return await _restaurantService.GetRestaurantInfoForSeller(new GetRestaurantInformationRequestDto { Id = id });
    }

    [HttpPatch("{id}/toggle-open")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> ToggleOpen(Guid id)
    {
        return await _restaurantService.ToggleOpen(id);
    }

    [HttpGet("{restaurantId}/working-hours")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<WorkingHourDto>> GetWorkingHours(Guid restaurantId)
    {
        return await _restaurantService.GetWorkingHours(restaurantId);
    }

    [HttpPut("{restaurantId}/working-hours")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpsertWorkingHour(Guid restaurantId, [FromBody] UpsertWorkingHourDto requestDto)
    {
        requestDto.RestaurantId = restaurantId;
        return await _restaurantService.UpsertWorkingHour(requestDto);
    }
}