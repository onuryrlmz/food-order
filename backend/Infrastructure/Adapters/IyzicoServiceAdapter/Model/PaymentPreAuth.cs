using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class PaymentPreAuth : PaymentResource
{
    public static PaymentPreAuth Create(CreatePaymentRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PaymentPreAuth>(options.BaseUrl + "/payment/preauth", GetHttpHeaders(request, options), request);
    }

    public static PaymentPreAuth Retrieve(RetrievePaymentRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PaymentPreAuth>(options.BaseUrl + "/payment/detail", GetHttpHeaders(request, options), request);
    }
}