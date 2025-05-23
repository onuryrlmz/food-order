using Application.Services.Seller._3_ProductService;
using Base.Enums;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/product")]
[ApiController]
public class ProductController : BaseController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] CreateProductDto requestDto)
    {
        return await _productService.CreateProduct(requestDto);
    }

    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductDto requestDto)
    {
        return await _productService.UpdateProduct(requestDto);
    }

    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductDto requestDto)
    {
        return await _productService.DeleteProduct(requestDto);
    }

    [HttpPost("getProductsByRestaurantId")]
    public async Task<ServiceCollectionResult<ProductResponseDto>> GetProductsByRestaurantId([FromBody] GetProductsRequestDto requestDto)
    {
        return await _productService.GetProducts(requestDto);
    }
}