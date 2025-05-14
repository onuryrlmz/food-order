using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class ApiTest : IyzipayResource
{
    public static IyzipayResource Retrieve(Options options)
    {
        return RestHttpClient.Create().Get<IyzipayResource>(options.BaseUrl + "/payment/test");
    }
}