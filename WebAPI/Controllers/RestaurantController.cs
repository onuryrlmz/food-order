using Application.Services.Seller._2_RestaurantService;
using Application.Services.Seller._99_RestaurantTransferService;
using Base.Enums;
using Domain.Dto.Seller.Restaurant;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/restaurant")]
[ApiController]
public class RestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;
    private readonly IRestaurantTransferService _restaurantTransferService;

    public RestaurantController(IRestaurantService restaurantService, IRestaurantTransferService restaurantTransferService)
    {
        _restaurantService = restaurantService;
        _restaurantTransferService = restaurantTransferService;
    }

    [HttpPost("add")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddRestaurantDto requestDto)
    {
        return await _restaurantService.AddRestaurant(requestDto);
    }

    [HttpPost("GetRestaurantInfo")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.User, AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<string>> GetRestaurantInfo([FromBody] GetRestaurantInformationRequestDto requestDto)
    {
        return await _restaurantService.GetRestaurantInfo(requestDto);
    }

    [HttpPost("GetRestaurantDataFromGetir")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task GetRestaurantDataFromGetir([FromQuery] string getirRestaurantId, [FromQuery] Guid restaurantId)
    {
        await _restaurantTransferService.SaveData(getirRestaurantId, restaurantId);
    }
}