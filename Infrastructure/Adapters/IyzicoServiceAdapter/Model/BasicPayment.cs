using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BasicPayment : BasicPaymentResource
{
    public static BasicPayment Create(CreateBasicPaymentRequest request, Options options)
    {
        return RestHttpClient.Create().Post<BasicPayment>(options.BaseUrl + "/payment/auth/basic", GetHttpHeaders(request, options), request);
    }
}