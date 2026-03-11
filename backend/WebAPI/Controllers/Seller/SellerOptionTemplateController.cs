using Application.Services.Seller.OptionTemplateService;
using Base.Enums;
using Domain.Dto.Seller.OptionTemplate;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/option-template")]
[ApiController]
public class SellerOptionTemplateController : BaseController
{
    private readonly IOptionTemplateService _optionTemplateService;

    public SellerOptionTemplateController(IOptionTemplateService optionTemplateService) => _optionTemplateService = optionTemplateService;

    [HttpGet("by-restaurant/{restaurantId}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceCollectionResult<OptionTemplateResponseDto>> GetByRestaurant(Guid restaurantId)
        => await _optionTemplateService.GetTemplatesByRestaurantId(restaurantId);

    [HttpGet("{id}")]
    [AuthorizeAPIRequest(true, false,
        AuthorizationServiceEnums.UserRoleEnums.SellerAdmin,
        AuthorizationServiceEnums.UserRoleEnums.SellerUser)]
    public async Task<ServiceObjectResult<OptionTemplateResponseDto>> GetById(Guid id)
        => await _optionTemplateService.GetTemplateById(id);

    [HttpPost]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> Create([FromBody] CreateOptionTemplateRequestDto requestDto)
        => await _optionTemplateService.CreateTemplate(requestDto);

    [HttpPut]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Update([FromBody] UpdateOptionTemplateRequestDto requestDto)
        => await _optionTemplateService.UpdateTemplate(requestDto);

    [HttpDelete]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> Delete([FromBody] DeleteOptionTemplateRequestDto requestDto)
        => await _optionTemplateService.DeleteTemplate(requestDto);

    // Template Value endpoints
    [HttpPost("value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> AddValue([FromBody] AddOptionTemplateValueRequestDto requestDto)
        => await _optionTemplateService.AddTemplateValue(requestDto);

    [HttpPut("value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateValue([FromBody] UpdateOptionTemplateValueRequestDto requestDto)
        => await _optionTemplateService.UpdateTemplateValue(requestDto);

    [HttpDelete("value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> DeleteValue([FromBody] DeleteOptionTemplateValueRequestDto requestDto)
        => await _optionTemplateService.DeleteTemplateValue(requestDto);

    // Template ValueOption endpoints
    [HttpPost("value-option")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> AddValueOption([FromBody] AddOptionTemplateValueOptionRequestDto requestDto)
        => await _optionTemplateService.AddTemplateValueOption(requestDto);

    [HttpPut("value-option")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateValueOption([FromBody] UpdateOptionTemplateValueOptionRequestDto requestDto)
        => await _optionTemplateService.UpdateTemplateValueOption(requestDto);

    [HttpDelete("value-option")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> DeleteValueOption([FromBody] DeleteOptionTemplateValueOptionRequestDto requestDto)
        => await _optionTemplateService.DeleteTemplateValueOption(requestDto);

    // Template ValueOptionValue endpoints
    [HttpPost("value-option-value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<Guid>> AddValueOptionValue([FromBody] AddOptionTemplateValueOptionValueRequestDto requestDto)
        => await _optionTemplateService.AddTemplateValueOptionValue(requestDto);

    [HttpPut("value-option-value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> UpdateValueOptionValue([FromBody] UpdateOptionTemplateValueOptionValueRequestDto requestDto)
        => await _optionTemplateService.UpdateTemplateValueOptionValue(requestDto);

    [HttpDelete("value-option-value")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.SellerAdmin)]
    public async Task<ServiceObjectResult<bool>> DeleteValueOptionValue([FromBody] DeleteOptionTemplateValueOptionValueRequestDto requestDto)
        => await _optionTemplateService.DeleteTemplateValueOptionValue(requestDto);
}
