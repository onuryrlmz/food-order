using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;
using Newtonsoft.Json;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BasicThreedsInitialize : IyzipayResource
{
    [JsonProperty(PropertyName = "threeDSHtmlContent")]
    public string HtmlContent { get; set; }

    public static BasicThreedsInitialize Create(CreateBasicPaymentRequest request, Options options)
    {
        var response = RestHttpClient.Create().Post<BasicThreedsInitialize>(options.BaseUrl + "/payment/3dsecure/initialize/basic", GetHttpHeaders(request, options), request);

        if (response != null) response.HtmlContent = DigestHelper.DecodeString(response.HtmlContent);
        return response;
    }
}