using Application.Services.Seller._9_CategoryService;
using Domain.Dto.Seller.Category;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/category")]
[ApiController]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("getCategoriesByRestaurantId")]
    public async Task<ServiceCollectionResult<CategoryResponse>> GetCategoriesByRestaurantId([FromBody] GetCategoriesByRestaurantIdRequestDto requestDto)
    {
        return await _categoryService.GetCategoriesByRestaurantId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateCategoryRequestDto requestDto)
    {
        return await _categoryService.CreateCategory(requestDto);
    }

    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateCategoryRequestDto requestDto)
    {
        return await _categoryService.UpdateCategory(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteCategoryRequestDto requestDto)
    {
        return await _categoryService.DeleteCategory(requestDto);
    }
}