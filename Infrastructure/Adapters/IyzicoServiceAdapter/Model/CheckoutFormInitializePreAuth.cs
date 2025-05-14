using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CheckoutFormInitializePreAuth : CheckoutFormInitializeResource
{
    public static CheckoutFormInitializePreAuth Create(CreateCheckoutFormInitializeRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CheckoutFormInitializePreAuth>(options.BaseUrl + "/payment/iyzipos/checkoutform/initialize/preauth/ecom", GetHttpHeaders(request, options), request);
    }
}