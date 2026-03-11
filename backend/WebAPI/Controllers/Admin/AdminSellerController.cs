using Application.Services.Seller._1_SellerService;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Admin;

[Route("v1/admin/seller")]
[ApiController]
public class AdminSellerController : BaseController
{
    private readonly ISellerService _sellerService;

    public AdminSellerController(ISellerService sellerService) => _sellerService = sellerService;

    [HttpGet("list")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceCollectionResult<GetSellerListResponseDto>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => await _sellerService.GetSellerList(page, pageSize);

    [HttpPost("add")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Add([FromBody] AddSellerDto requestDto)
        => await _sellerService.AddSeller(requestDto);

    [HttpPost("confirm")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Confirm([FromBody] ConfirmSellerDto requestDto)
        => await _sellerService.ConfirmSeller(requestDto);

    [HttpPut("{id}")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<ServiceObjectResult<bool>> Update(Guid id, [FromBody] UpdateSellerDto requestDto)
    {
        requestDto.Id = id;
        return await _sellerService.UpdateSeller(requestDto);
    }
}
