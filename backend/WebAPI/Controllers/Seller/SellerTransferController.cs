using Application.Services.Seller._99_RestaurantTransferService;
using Base.Enums;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Seller;

[Route("v1/seller/transfer")]
[ApiController]
public class SellerTransferController : BaseController
{
    private readonly IRestaurantTransferService _transferService;

    public SellerTransferController(IRestaurantTransferService transferService)
        => _transferService = transferService;

    [HttpPost("getir")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<IActionResult> TransferFromGetir([FromBody] TransferRequestDto request)
    {
        try
        {
            await _transferService.TransferDataFromGetir(request.SourceRestaurantId, request.TargetRestaurantId);
            return Ok(new { hasFailed = false, data = "Getir transfer completed successfully." });
        }
        catch (Exception e)
        {
            return Ok(new { hasFailed = true, messages = new[] { new { description = e.Message } } });
        }
    }

    [HttpPost("yemeksepeti")]
    [AuthorizeAPIRequest(true, false, AuthorizationServiceEnums.UserRoleEnums.Admin)]
    public async Task<IActionResult> TransferFromYemekSepeti([FromBody] TransferRequestDto request)
    {
        try
        {
            await _transferService.TransferDataFromYemekSepeti(request.SourceRestaurantId, request.TargetRestaurantId);
            return Ok(new { hasFailed = false, data = "YemekSepeti transfer completed successfully." });
        }
        catch (Exception e)
        {
            return Ok(new { hasFailed = true, messages = new[] { new { description = e.Message } } });
        }
    }
}

public class TransferRequestDto
{
    public string SourceRestaurantId { get; set; } = string.Empty;
    public Guid TargetRestaurantId { get; set; }
}
