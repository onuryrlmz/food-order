using Infrastructure.Adapters.IyzicoServiceAdapter;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Base;

[Route("v1/payment")]
[ApiController]
public class PaymentCallbackController : BaseController
{
    private readonly IIyzicoServiceAdapter _iyzicoServiceAdapter;

    public PaymentCallbackController(IIyzicoServiceAdapter iyzicoServiceAdapter) => _iyzicoServiceAdapter = iyzicoServiceAdapter;

    [HttpPost("iyzico/callback")]
    public async Task<IActionResult> IyzicoCallback([FromForm] string? conversationId, [FromForm] string? paymentId, [FromForm] string? conversationData, [FromForm] string? status)
    {
        if (status != "success" || string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(conversationData))
        {
            return BadRequest(new { success = false, message = "Payment failed or missing parameters." });
        }

        var result = await _iyzicoServiceAdapter.CompleteThreeDsPayment(conversationId, paymentId, conversationData);
        if (result.HasFailed)
            return BadRequest(result);

        return Ok(result);
    }
}
