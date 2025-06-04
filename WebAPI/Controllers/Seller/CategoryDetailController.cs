using Application.Services.Seller._10_CategoryDetailService;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/categoryDetail")]
[ApiController]
public class CategoryDetailController : BaseController
{
    private readonly ICategoryDetailService _categoryDetailService;

    public CategoryDetailController(ICategoryDetailService categoryDetailService)
    {
        _categoryDetailService = categoryDetailService;
    }

    [HttpPost("getCategoryDetailsByCategoryId")]
    public async Task<ServiceCollectionResult<CategoryDetailResponse>> GetCategoryDetailsByCategoryId([FromBody] GetCategoryDetailsByCategoryIdRequestDto requestDto)
    {
        return await _categoryDetailService.GetCategoryDetailsByCategoryId(requestDto);
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateCategoryDetailRequestDto requestDto)
    {
        return await _categoryDetailService.CreateCategoryDetail(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteCategoryDetailRequestDto requestDto)
    {
        return await _categoryDetailService.DeleteCategoryDetail(requestDto);
    }
}