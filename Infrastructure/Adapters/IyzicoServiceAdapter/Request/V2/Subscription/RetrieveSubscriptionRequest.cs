using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class RetrieveSubscriptionRequest : BaseRequestV2
{
    public string SubscriptionReferenceCode { get; set; }
}