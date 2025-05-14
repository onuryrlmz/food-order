using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2.Transaction;

public class TransactionDetail : TransactionDetailResource
{
    public static TransactionDetail Retrieve(RetrieveTransactionDetailRequest request, Options options)
    {
        string url;
        if (string.IsNullOrEmpty(request.PaymentId))
            url = options.BaseUrl
                  + "/v2/reporting/payment/details?paymentConversationId="
                  + request.PaymentConversationId;
        else
            url = options.BaseUrl
                  + "/v2/reporting/payment/details?paymentId="
                  + request.PaymentId;
        return RestHttpClientV2.Create().Get<TransactionDetail>(url, GetHttpHeadersWithUrlParams(request, url, options));
    }
}