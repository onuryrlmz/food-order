using Application.Services.Seller;
using Base.Enums;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/restaurant")]
[ApiController]
public class RestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpPost("add")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddRestaurantDto requestDto)
    {
        return await _restaurantService.AddRestaurant(requestDto);
    }

    [HttpPost("createRestaurantInformationJsonFile")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<bool>> CreateRestaurantInformationJsonFile([FromBody] CreateRestaurantInformationJsonFileRequestDto requestDto)
    {
        return await _restaurantService.CreateRestaurantInformationJsonFile(requestDto);
    }

    // [HttpPost("getRestaurantsBySellerId")]
    // [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    // public async Task<ServiceCollectionResult<GetRestaurantsBySellerIdDto>> GetRestaurantsBySellerId([FromBody] GetRestaurantsBySellerIdQuery getRestaurantBySellerIdQuery)
    // {
    //     var request = new MediatRRequest<GetRestaurantsBySellerIdQuery, ServiceCollectionResult<GetRestaurantsBySellerIdDto>>(getRestaurantBySellerIdQuery, Client);
    //     return await Mediator.Send(request);
    // }

    [HttpPost("getRestaurantInformationByRestaurantId")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.User })]
    public async Task<ServiceObjectResult<RestaurantResponseDto>> GetRestaurantInformationByRestaurantId([FromBody] GetRestaurantInformationRequestDto getRestaurantInformationByRestaurantIdQuery)
    {
        return await _restaurantService.GetRestaurantInformationByRestaurantId(getRestaurantInformationByRestaurantIdQuery);
    }
}