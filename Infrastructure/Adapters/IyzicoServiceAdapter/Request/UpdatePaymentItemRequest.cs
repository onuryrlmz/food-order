using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class UpdatePaymentItemRequest : BaseRequest
{
    public string SubMerchantKey { get; set; }
    public string PaymentTransactionId { get; set; }
    public string SubMerchantPrice { get; set; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("subMerchantKey", SubMerchantKey)
            .Append("paymentTransactionId", PaymentTransactionId)
            .Append("subMerchantPrice", SubMerchantPrice)
            .GetRequestString();
    }
}