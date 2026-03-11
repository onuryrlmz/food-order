using Application.Services.Seller._10_CategoryDetailService;
using Base.Enums;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/category-detail")]
[ApiController]
public class SellerCategoryDetailController : BaseController
{
    private readonly ICategoryDetailService _categoryDetailService;

    public SellerCategoryDetailController(ICategoryDetailService categoryDetailService) => _categoryDetailService = categoryDetailService;

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<CategoryDetailResponse>> GetList([FromQuery] GetCategoryDetailsByCategoryIdRequestDto requestDto)
        => await _categoryDetailService.GetCategoryDetailsByCategoryId(requestDto);

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateCategoryDetailRequestDto requestDto)
        => await _categoryDetailService.CreateCategoryDetail(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteCategoryDetailRequestDto requestDto)
        => await _categoryDetailService.DeleteCategoryDetail(requestDto);
}
