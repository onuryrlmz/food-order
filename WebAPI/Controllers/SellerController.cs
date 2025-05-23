using Application.Services.Seller._1_SellerService;
using Base.Enums;
using Domain.Dto.Common;
using Domain.Dto.Seller;
using Domain.Service;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class SellerController : BaseController
{
    private readonly ISellerService _sellerService;

    public SellerController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpPost("add")]
    public async Task<ServiceObjectResult<bool>> Add([FromBody] AddSellerDto addSellerDto)
    {
        return await _sellerService.AddSeller(addSellerDto);
    }

    [HttpPost("confirm")]
    [AuthorizeAPIRequest(true, false, new[] { AuthorizationServiceEnums.UserRoleEnums.Admin })]
    public async Task<ServiceObjectResult<bool>> Confirm([FromBody] ConfirmSellerDto confirmSellerCommand)
    {
        return await _sellerService.ConfirmSeller(confirmSellerCommand);
    }
}