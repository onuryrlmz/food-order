using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class ActivateSubscriptionRequest : BaseRequestV2
{
    public string SubscriptionReferenceCode { get; set; }
}