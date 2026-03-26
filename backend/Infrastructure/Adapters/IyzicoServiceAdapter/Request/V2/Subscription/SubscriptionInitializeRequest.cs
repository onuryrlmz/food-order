using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class SubscriptionInitializeRequest : BaseRequestV2
{
    public string PricingPlanReferenceCode { get; set; }
    public string SubscriptionInitialStatus { get; set; }
    public CheckoutFormCustomer Customer { get; set; }
    public CardInfo PaymentCard { get; set; }
}
