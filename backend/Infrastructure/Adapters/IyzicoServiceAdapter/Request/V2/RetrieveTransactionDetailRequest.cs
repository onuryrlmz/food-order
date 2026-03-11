using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2;

public class RetrieveTransactionDetailRequest : BaseRequestV2
{
    public string PaymentConversationId { get; set; }
    public string PaymentId { get; set; }
}