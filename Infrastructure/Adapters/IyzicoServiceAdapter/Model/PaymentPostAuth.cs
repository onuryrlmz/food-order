using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class PaymentPostAuth : PaymentResource
{
    public static PaymentPostAuth Create(CreatePaymentPostAuthRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PaymentPostAuth>(options.BaseUrl + "/payment/postauth", GetHttpHeaders(request, options), request);
    }
}