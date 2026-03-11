using Application.Services.Seller._0_CuisineService;
using Base.Enums;
using Domain.Dto.Seller.Cuisine;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Base;

[Route("v1/cuisine")]
[ApiController]
public class CuisineController : BaseController
{
    private readonly ICuisineService _cuisineService;

    public CuisineController(ICuisineService cuisineService) => _cuisineService = cuisineService;

    [HttpGet]
    public async Task<ServiceCollectionResult<CuisineResponseDto>> GetList()
        => await _cuisineService.GetList();

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Add([FromBody] AddCuisineDto requestDto)
        => await _cuisineService.Add(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateCuisineDto requestDto)
        => await _cuisineService.Update(requestDto);

    [HttpDelete("{id}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Delete(Guid id)
        => await _cuisineService.Delete(id);
}
