using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class PayWithIyzicoInitialize : PayWithIyzicoInitializeResource
{
    public static PayWithIyzicoInitialize Create(CreatePayWithIyzicoInitializeRequest request, Options options)
    {
        return RestHttpClient.Create().Post<PayWithIyzicoInitialize>(options.BaseUrl + "/payment/pay-with-iyzico/initialize", GetHttpHeaders(request, options), request);
    }
}