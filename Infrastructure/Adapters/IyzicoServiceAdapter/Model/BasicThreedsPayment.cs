using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BasicThreedsPayment : BasicPaymentResource
{
    public static BasicThreedsPayment Create(CreateThreedsPaymentRequest request, Options options)
    {
        return RestHttpClient.Create().Post<BasicThreedsPayment>(options.BaseUrl + "/payment/3dsecure/auth/basic", GetHttpHeaders(request, options), request);
    }
}