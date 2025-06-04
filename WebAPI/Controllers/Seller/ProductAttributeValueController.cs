using Application.Services.Seller._5_ProductAttributeValueService;
using Base.Enums;
using Domain.Dto.Seller.ProductAttributeValue;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/productAttributeValue")]
[ApiController]
public class ProductAttributeValueController : BaseController
{
    private readonly IProductAttributeValueService _productAttributeValueService;

    public ProductAttributeValueController(IProductAttributeValueService productAttributeValueService)
    {
        _productAttributeValueService = productAttributeValueService;
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("getProductAttributeValues")]
    public async Task<ServiceCollectionResult<GetProductAttributeValueResponseDto>> GetProductAttributeValues([FromBody] GetProductAttributeValuesRequestDto requestDto)
    {
        return await _productAttributeValueService.GetProductAttributeValues(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpPost("add")]
    public async Task<ServiceObjectResult<Guid>> Add([FromBody] AddProductAttributeValueDto requestDto)
    {
        return await _productAttributeValueService.Add(requestDto);
    }

    [AuthorizeAPIRequest(true, false, [AuthorizationServiceEnums.UserRoleEnums.Admin, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin, AuthorizationServiceEnums.UserRoleEnums.SellerUser])]
    [HttpDelete("delete")]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteProductAttributeValueDto requestDto)
    {
        return await _productAttributeValueService.Delete(requestDto);
    }
}