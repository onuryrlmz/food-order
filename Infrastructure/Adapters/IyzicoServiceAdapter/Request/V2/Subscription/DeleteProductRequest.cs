using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class DeleteProductRequest : BaseRequestV2
{
    public string ProductReferenceCode { get; set; }
}