using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class Approval : IyzipayResource
{
    public string PaymentTransactionId { get; set; }

    public static Approval Create(CreateApprovalRequest request, Options options)
    {
        return RestHttpClient.Create().Post<Approval>(options.BaseUrl + "/payment/iyzipos/item/approve", GetHttpHeaders(request, options), request);
    }
}