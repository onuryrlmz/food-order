using Newtonsoft.Json;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Subscription;

public class SubscriptionResource : SubscriptionCreatedResource
{
    public string CustomerEmail { get; set; }

    [JsonProperty(PropertyName = "orders")]
    public List<SubscriptionOrder> SubscriptionOrders { get; set; }
}