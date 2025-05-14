using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class RetrieveCustomerRequest : BaseRequestV2
{
    public string CustomerReferenceCode { get; set; }
}