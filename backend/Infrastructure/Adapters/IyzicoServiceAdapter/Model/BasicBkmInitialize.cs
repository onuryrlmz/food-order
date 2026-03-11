using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BasicBkmInitialize : IyzipayResource
{
    public string HtmlContent { get; set; }
    public string Token { get; set; }

    public static BasicBkmInitialize Create(CreateBasicBkmInitializeRequest request, Options options)
    {
        var response = RestHttpClient.Create().Post<BasicBkmInitialize>(options.BaseUrl + "/payment/bkm/initialize/basic", GetHttpHeaders(request, options), request);

        if (response != null) response.HtmlContent = DigestHelper.DecodeString(response.HtmlContent);
        return response;
    }
}