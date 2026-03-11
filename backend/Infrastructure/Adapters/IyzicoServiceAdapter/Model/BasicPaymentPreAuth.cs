using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BasicPaymentPreAuth : BasicPaymentResource
{
    public static BasicPaymentPreAuth Create(CreateBasicPaymentRequest request, Options options)
    {
        return RestHttpClient.Create().Post<BasicPaymentPreAuth>(options.BaseUrl + "/payment/preauth/basic", GetHttpHeaders(request, options), request);
    }
}