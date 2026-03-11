using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class Bkm : PaymentResource
{
    public string Token { get; set; }
    public string CallbackUrl { get; set; }

    public static Bkm Retrieve(RetrieveBkmRequest request, Options options)
    {
        return RestHttpClient.Create().Post<Bkm>(options.BaseUrl + "/payment/bkm/auth/detail", GetHttpHeaders(request, options), request);
    }
}