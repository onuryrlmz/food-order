using Application.Services.Seller.CategoryDetailService;
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

    public SellerCategoryDetailController(ICategoryDetailService categoryDetailService)
    {
        _categoryDetailService = categoryDetailService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        UserRoleEnums.SellerAdmin,
        UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<CategoryDetailResponse>> GetList([FromQuery] GetCategoryDetailsByCategoryIdRequestDto requestDto)
    {
        return await _categoryDetailService.GetCategoryDetailsByCategoryId(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateCategoryDetailRequestDto requestDto)
    {
        return await _categoryDetailService.CreateCategoryDetail(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteCategoryDetailRequestDto requestDto)
    {
        return await _categoryDetailService.DeleteCategoryDetail(requestDto);
    }
}