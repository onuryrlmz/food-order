using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class DeletePlanRequest : BaseRequestV2
{
    public string PricingPlanReferenceCode { get; set; }
}