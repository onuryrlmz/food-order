using Application.Services.Seller._2_RestaurantService;
using Application.Services.Seller._99_RestaurantTransferService;
using Base.Enums;
using Domain.Dto.Seller.Restaurant;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/restaurant")]
[ApiController]
public class RestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;
    private readonly IRestaurantTransferServiceV2 _restaurantTransferServiceV2;

    public RestaurantController(IRestaurantService restaurantService, IRestaurantTransferServiceV2 restaurantTransferServiceV2)
    {
        _restaurantService = restaurantService;
        _restaurantTransferServiceV2 = restaurantTransferServiceV2;
    }

    [HttpPost("AddRestaurant")]
    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin])]
    public async Task<ServiceObjectResult<Guid>> AddRestaurant([FromBody] AddRestaurantDto requestDto)
    {
        return await _restaurantService.AddRestaurant(requestDto);
    }

    [HttpPost("GetRestaurantListForSeller")]
    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin])]
    public async Task<ServiceCollectionResult<GetRestaurantListForSellerResponseDto>> GetRestaurantListForSeller()
    {
        return await _restaurantService.GetRestaurantListForSeller();
    }

    [HttpPost("GetRestaurantInfoForSeller")]
    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin])]
    public async Task<ServiceObjectResult<string>> GetRestaurantInfoForSeller([FromBody] GetRestaurantInformationRequestDto requestDto)
    {
        return await _restaurantService.GetRestaurantInfoForSeller(requestDto);
    }

    [HttpPost("TransferDataFromGetir")]
    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin])]
    public async Task TransferDataFromGetir([FromQuery] string getirRestaurantId, [FromQuery] Guid restaurantId)
    {
        await _restaurantTransferServiceV2.TransferDataFromGetir(getirRestaurantId, restaurantId);
    }

    [HttpPost("TransferDataFromYemekSepeti")]
    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin])]
    public async Task TransferDataFromYemekSepeti([FromQuery] string ysRestaurantId, [FromQuery] Guid restaurantId)
    {
        await _restaurantTransferServiceV2.TransferDataFromYemekSepeti(ysRestaurantId, restaurantId);
    }
}