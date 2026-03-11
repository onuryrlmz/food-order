using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Transaction;

public class TransactionDetailResource : IyzipayResourceV2
{
    public List<TransactionDetailItem> Payments { get; set; }
}