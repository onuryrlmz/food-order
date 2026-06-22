using Application.Services.Seller.RestaurantService;
using Base.Enums;
using Domain.Dto.Seller.Restaurant;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/restaurant")]
[ApiController]
public class AdminRestaurantController : BaseController
{
    private readonly IRestaurantService _restaurantService;

    public AdminRestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetAdminRestaurantListResponseDto>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        return await _restaurantService.GetAllRestaurantsForAdmin(page, pageSize);
    }

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<GetAdminRestaurantListResponseDto>> GetById(Guid id)
    {
        return await _restaurantService.GetRestaurantByIdForAdmin(id);
    }

    [HttpPut("{id}")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Update(Guid id, [FromBody] UpdateAdminRestaurantDto requestDto)
    {
        requestDto.Id = id;
        return await _restaurantService.UpdateRestaurantForAdmin(requestDto);
    }

    [HttpPatch("{id}/toggle-active")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> ToggleActive(Guid id)
    {
        return await _restaurantService.ToggleActive(id);
    }

    [HttpPost("{id}/approve")]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Approve(Guid id)
    {
        return await _restaurantService.ApproveRestaurant(id);
    }
}