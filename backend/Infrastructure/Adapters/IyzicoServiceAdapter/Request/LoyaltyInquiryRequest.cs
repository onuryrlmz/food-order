using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Model;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class LoyaltyInquiryRequest : BaseRequest
{
    public LoyaltyPaymentCard PaymentCard { set; get; }
    public string Currency { set; get; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("paymentCard", PaymentCard.ToPKIRequestString())
            .Append("currency", Currency)
            .GetRequestString();
    }
}