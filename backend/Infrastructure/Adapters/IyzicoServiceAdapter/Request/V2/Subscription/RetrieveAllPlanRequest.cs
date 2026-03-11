using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class RetrieveAllPlanRequest : PagingRequest
{
    public string ProductReferenceCode { get; set; }
}