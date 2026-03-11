using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class RetrieveApmRequest : BaseRequest
{
    public string PaymentId { get; set; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("paymentId", PaymentId)
            .GetRequestString();
    }
}