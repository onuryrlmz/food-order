using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class PayoutCompletedTransactionList : IyzipayResource
{
    public List<PayoutCompletedTransaction> PayoutCompletedTransactions { get; set; }

    public static PayoutCompletedTransactionList Retrieve(RetrieveTransactionsRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PayoutCompletedTransactionList>(options.BaseUrl + "/reporting/settlement/payoutcompleted", GetHttpHeaders(request, options), request);
    }
}