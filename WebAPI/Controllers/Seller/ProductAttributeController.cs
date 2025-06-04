using Application.Services.Seller._4_ProductAttributeService;
using Base.Enums;
using Domain.Dto.Seller.ProductAttribute;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/productAttribute")]
[ApiController]
public class ProductAttributeController : BaseController
{
    private readonly IProductAttributeService _productAttributeService;

    public ProductAttributeController(IProductAttributeService productAttributeService)
    {
        _productAttributeService = productAttributeService;
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("getProductAttributes")]
    public async Task<ServiceCollectionResult<GetProductAttributeResponseDto>> GetProductAttributes([FromBody] GetProductAttributesRequestDto requestDto)
    {
        return await _productAttributeService.GetProductAttributes(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddProductAttributeDto requestDto)
    {
        return await _productAttributeService.Add(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPut("update")]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductAttributeDto requestDto)
    {
        return await _productAttributeService.Update(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductAttributeDto requestDto)
    {
        return await _productAttributeService.Delete(requestDto);
    }
}