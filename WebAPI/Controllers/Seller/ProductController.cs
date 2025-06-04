using Application.Services.Seller._3_ProductService;
using Base.Enums;
using Domain.Dto.Seller.Product;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/product")]
[ApiController]
public class ProductController : BaseController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddProductDto requestDto)
    {
        return await _productService.CreateProduct(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductDto requestDto)
    {
        return await _productService.UpdateProduct(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductDto requestDto)
    {
        return await _productService.DeleteProduct(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("getProductsByRestaurantId")]
    public async Task<ServiceCollectionResult<ProductResponseDto>> GetProductsByRestaurantId([FromBody] GetProductsRequestDto requestDto)
    {
        return await _productService.GetProducts(requestDto);
    }
}