using Application.Services.Seller.CategoryService;
using Base.Enums;
using Domain.Dto.Seller.Category;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/category")]
[ApiController]
public class SellerCategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public SellerCategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<CategoryResponse>> GetList([FromQuery] GetCategoriesByRestaurantIdRequestDto requestDto)
    {
        return await _categoryService.GetCategoriesByRestaurantId(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateCategoryRequestDto requestDto)
    {
        return await _categoryService.CreateCategory(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateCategoryRequestDto requestDto)
    {
        return await _categoryService.UpdateCategory(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteCategoryRequestDto requestDto)
    {
        return await _categoryService.DeleteCategory(requestDto);
    }
}