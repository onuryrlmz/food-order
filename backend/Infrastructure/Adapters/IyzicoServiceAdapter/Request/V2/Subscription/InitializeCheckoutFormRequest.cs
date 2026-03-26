using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Model;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class InitializeCheckoutFormRequest : BaseRequestV2
{
    public string CallbackUrl { get; set; }
    public string PricingPlanReferenceCode { get; set; }
    public string SubscriptionInitialStatus { get; set; }
    public CheckoutFormCustomer Customer { get; set; }
}
