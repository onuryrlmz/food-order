using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class Disapproval : IyzipayResource
{
    public string PaymentTransactionId { get; set; }

    public static Disapproval Create(CreateApprovalRequest request, Options options)
    {
        return RestHttpClient.Create().Post<Disapproval>(options.BaseUrl + "/payment/iyzipos/item/disapprove", GetHttpHeaders(request, options), request);
    }
}