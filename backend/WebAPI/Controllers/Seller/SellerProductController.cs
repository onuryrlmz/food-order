using Application.Services.Seller.ProductService;
using Base.Enums;
using Domain.Dto.Seller.Product;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/product")]
[ApiController]
public class SellerProductController : BaseController
{
    private readonly IProductService _productService;

    public SellerProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<ProductResponseDto>> GetList([FromQuery] GetProductsRequestDto requestDto)
    {
        return await _productService.GetProducts(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] AddProductDto requestDto)
    {
        return await _productService.CreateProduct(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductDto requestDto)
    {
        return await _productService.UpdateProduct(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductDto requestDto)
    {
        return await _productService.DeleteProduct(requestDto);
    }
}