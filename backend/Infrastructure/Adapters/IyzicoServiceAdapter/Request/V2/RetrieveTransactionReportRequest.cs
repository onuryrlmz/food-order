using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2;

public class RetrieveTransactionReportRequest : BaseRequestV2
{
    public string TransactionDate { get; set; }
    public int Page { get; set; }
}