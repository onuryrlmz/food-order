using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CheckoutForm : PaymentResource
{
    public string Token { get; set; }
    public string CallbackUrl { get; set; }

    public static CheckoutForm Retrieve(RetrieveCheckoutFormRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CheckoutForm>(options.BaseUrl + "/payment/iyzipos/checkoutform/auth/ecom/detail", GetHttpHeaders(request, options), request);
    }
}