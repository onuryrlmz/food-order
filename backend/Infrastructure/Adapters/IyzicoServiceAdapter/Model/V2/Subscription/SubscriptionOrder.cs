using Newtonsoft.Json;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Subscription;

public class SubscriptionOrder
{
    public string ReferenceCode { get; set; }
    public string Price { get; set; }
    public string CurrencyCode { get; set; }
    public string StartPeriod { get; set; }
    public string EndPeriod { get; set; }
    public string OrderStatus { get; set; }

    [JsonProperty(PropertyName = "paymentAttempts")]
    public List<PaymentAttemptDto> OrderPaymentAttempts { get; set; }
}
