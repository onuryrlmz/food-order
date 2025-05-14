using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class RetrievePlanRequest : BaseRequestV2
{
    public string PricingPlanReferenceCode { get; set; }
}