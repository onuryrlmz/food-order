using Application.Services.Seller.ProductAttributeService;
using Application.Services.Seller.ProductAttributeValueService;
using Base.Enums;
using Domain.Dto.Seller.ProductAttribute;
using Domain.Dto.Seller.ProductAttributeValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/product-attribute")]
[ApiController]
public class SellerProductAttributeController : BaseController
{
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductAttributeValueService _productAttributeValueService;

    public SellerProductAttributeController(IProductAttributeService productAttributeService, IProductAttributeValueService productAttributeValueService)
    {
        _productAttributeService = productAttributeService;
        _productAttributeValueService = productAttributeValueService;
    }

    [HttpGet]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetProductAttributeResponseDto>> GetList([FromQuery] GetProductAttributesRequestDto requestDto)
    {
        return await _productAttributeService.GetProductAttributes(requestDto);
    }

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddProductAttributeDto requestDto)
    {
        return await _productAttributeService.Add(requestDto);
    }

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateProductAttributeDto requestDto)
    {
        return await _productAttributeService.Update(requestDto);
    }

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductAttributeDto requestDto)
    {
        return await _productAttributeService.Delete(requestDto);
    }

    // Attribute Values
    [HttpGet("values")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<GetProductAttributeValueResponseDto>> GetValues([FromQuery] GetProductAttributeValuesRequestDto requestDto)
    {
        return await _productAttributeValueService.GetProductAttributeValues(requestDto);
    }

    [HttpPost("values")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> AddValue([FromBody] AddProductAttributeValueDto requestDto)
    {
        return await _productAttributeValueService.Add(requestDto);
    }

    [HttpDelete("values")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> DeleteValue([FromBody] DeleteProductAttributeValueDto requestDto)
    {
        return await _productAttributeValueService.Delete(requestDto);
    }
}