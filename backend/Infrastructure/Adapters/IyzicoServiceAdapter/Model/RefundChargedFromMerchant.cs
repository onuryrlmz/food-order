using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class RefundChargedFromMerchant : IyzipayResource
{
    public string PaymentId { get; set; }
    public string PaymentTransactionId { get; set; }
    public string Price { get; set; }
    public string AuthCode { get; set; }
    public string HostReference { get; set; }

    public static RefundChargedFromMerchant Create(CreateRefundRequest request, Options options)
    {
        return RestHttpClient.Create().Post<RefundChargedFromMerchant>(options.BaseUrl + "/payment/iyzipos/refund/merchant/charge", GetHttpHeaders(request, options), request);
    }
}