using Application.Services.Buyer.PaymentService;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebAPI.Controllers.Base;

[Route("v1/payment")]
[ApiController]
public class PaymentCallbackController : BaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentCallbackController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("iyzico/callback")]
    public async Task<IActionResult> IyzicoCallback([FromForm] IFormCollection form)
    {
        var status = form["status"].FirstOrDefault();
        var conversationId = form["conversationId"].FirstOrDefault();
        var paymentId = form["paymentId"].FirstOrDefault();
        var conversationData = form["conversationData"].FirstOrDefault();

        if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(paymentId))
        {
            var failHtml = GenerateCallbackHtml(false, "", "Ödeme başarısız oldu. Eksik parametre.");
            return Content(failHtml, "text/html");
        }

        var isSuccess = status == "success";
        var errorMsg = isSuccess ? null : $"Ödeme başarısız. status={status}";

        var result = await _paymentService.HandlePaymentCallback(conversationId, paymentId, conversationData ?? "", isSuccess, errorMsg);

        if (!result.HasFailed)
        {
            var successHtml = GenerateCallbackHtml(true, conversationId, "Ödeme başarılı!");
            return Content(successHtml, "text/html");
        }
        else
        {
            var errDescription = result.Messages?.FirstOrDefault()?.Description ?? "Ödeme tamamlanamadı.";
            var errorHtml = GenerateCallbackHtml(false, conversationId, errDescription);
            return Content(errorHtml, "text/html");
        }
    }

    private static string GenerateCallbackHtml(bool success, string orderId, string message)
    {
        var statusText = success ? "success" : "failure";

        // XSS koruması: dinamik değerler script/HTML bağlamına escape edilmeden gömülmemeli.
        // JS nesnesini System.Text.Json ile üretiyoruz (HTML-duyarlı karakterleri de kaçırır),
        // <p> içindeki mesajı ise HTML-encode ediyoruz. conversationId Guid'e parse edilir.
        var safeOrderId = Guid.TryParse(orderId, out var parsed) ? parsed.ToString() : string.Empty;
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = statusText,
            orderId = safeOrderId,
            message
        });
        var htmlMessage = System.Net.WebUtility.HtmlEncode(message);

        return $@"<!DOCTYPE html>
<html>
<head><meta name=""viewport"" content=""width=device-width, initial-scale=1""></head>
<body>
<script>
  window.ReactNativeWebView && window.ReactNativeWebView.postMessage(JSON.stringify({payloadJson}));
</script>
<p style=""text-align:center;margin-top:40px;font-family:sans-serif;"">{htmlMessage}</p>
</body>
</html>";
    }
}