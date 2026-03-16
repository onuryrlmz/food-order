using Application.Services.Courier.CourierCompanyService;
using Base.Enums;
using Domain.Dto.Courier.Company;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller")]
[ApiController]
public class SellerCourierCompanyController : BaseController
{
    private readonly ICourierCompanyService _service;

    public SellerCourierCompanyController(ICourierCompanyService service) => _service = service;

    [HttpPost("restaurant/{restaurantId}/courier-company/add")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> InviteCompany(
        Guid restaurantId, [FromBody] InviteCompanyRequestDto request)
        => await _service.InviteCompanyToRestaurantAsync(restaurantId, request.CompanyId);

    [HttpGet("restaurant/{restaurantId}/courier-companies")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<RestaurantCourierCompanyDto>> GetCompanies(Guid restaurantId)
        => await _service.GetRestaurantCompaniesAsync(restaurantId);

    [HttpDelete("restaurant/{restaurantId}/courier-company/{companyId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<bool>> RemoveCompany(Guid restaurantId, Guid companyId)
        => await _service.RemoveCompanyFromRestaurantAsync(restaurantId, companyId);
}
