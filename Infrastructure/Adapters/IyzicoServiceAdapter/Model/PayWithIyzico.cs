using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class PayWithIyzico : PaymentResource
{
    public string Token { get; set; }
    public string CallbackUrl { get; set; }

    public static PayWithIyzico Retrieve(RetrievePayWithIyzicoRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PayWithIyzico>(options.BaseUrl + "/payment/iyzipos/checkoutform/auth/ecom/detail", GetHttpHeaders(request, options), request);
    }
}