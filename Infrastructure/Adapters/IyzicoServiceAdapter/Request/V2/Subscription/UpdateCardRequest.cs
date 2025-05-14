using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class UpdateCardRequest : BaseRequestV2
{
    public string CustomerReferenceCode { get; set; }
    public string CallbackUrl { get; set; }
}