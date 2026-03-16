using Application.Services.Buyer.FavoriteService;
using Base.Enums;
using Domain.Dto.Buyer.Favorite;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Customer;

[Route("v1/customer/favorite")]
[ApiController]
public class CustomerFavoriteController : BaseController
{
    private readonly IFavoriteService _favoriteService;

    public CustomerFavoriteController(IFavoriteService favoriteService) => _favoriteService = favoriteService;

    [HttpPost("{restaurantId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> Add(Guid restaurantId)
        => await _favoriteService.AddFavorite(restaurantId);

    [HttpDelete("{restaurantId}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<bool>> Remove(Guid restaurantId)
        => await _favoriteService.RemoveFavorite(restaurantId);

    [HttpGet]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceCollectionResult<FavoriteRestaurantDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => await _favoriteService.GetFavorites(page, pageSize);

    [HttpGet("ids")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.User)]
    public async Task<ServiceObjectResult<HashSet<Guid>>> GetIds()
        => await _favoriteService.GetFavoriteRestaurantIds();
}
