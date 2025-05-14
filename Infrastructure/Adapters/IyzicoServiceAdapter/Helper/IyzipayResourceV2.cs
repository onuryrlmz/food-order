using System.Net.Http.Headers;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

public class IyzipayResourceV2
{
    private static readonly string AUTHORIZATION = "Authorization";
    private static readonly string CONVERSATION_ID_HEADER_NAME = "x-conversation-id";
    private static readonly string CLIENT_VERSION_HEADER_NAME = "x-iyzi-client-version";
    private static readonly string IYZIWS_V2_HEADER_NAME = "IYZWSv2 ";

    public string Status { get; set; }
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }
    public string ConversationId { get; set; }
    public long SystemTime { get; set; }
    public string Locale { get; set; }

    public void AppendWithHttpResponseHeaders(HttpResponseMessage httpResponseMessage)
    {
        HttpHeaders responseHeaders = httpResponseMessage.Headers;
        StatusCode = Convert.ToInt32(httpResponseMessage.StatusCode);

        IEnumerable<string> values;
        if (responseHeaders.TryGetValues(CONVERSATION_ID_HEADER_NAME, out values))
        {
            var conversationId = values.First();
            ConversationId = !string.IsNullOrWhiteSpace(conversationId) ? conversationId : null;
        }
    }

    protected static Dictionary<string, string> GetHttpHeadersWithRequestBody(BaseRequestV2 request, string url, Options options)
    {
        var headers = GetCommonHttpHeaders(request, url, options);
        headers.Add(AUTHORIZATION, PrepareAuthorizationStringWithRequestBody(request, url, options));
        return headers;
    }

    protected static Dictionary<string, string> GetHttpHeadersWithUrlParams(BaseRequestV2 request, string url, Options options)
    {
        var headers = GetCommonHttpHeaders(request, url, options);
        headers.Add(AUTHORIZATION, PrepareAuthorizationStringWithRequestBody(null, url, options));
        return headers;
    }

    private static Dictionary<string, string> GetCommonHttpHeaders(BaseRequestV2 request, string url, Options options)
    {
        var headers = new Dictionary<string, string>();
        headers.Add("Accept", "application/json");
        headers.Add(CLIENT_VERSION_HEADER_NAME, IyzipayConstants.CLIENT_VERSION);
        headers.Add(CONVERSATION_ID_HEADER_NAME, request.ConversationId);
        return headers;
    }

    private static string PrepareAuthorizationStringWithRequestBody(BaseRequestV2 request, string url, Options options)
    {
        var randomKey = GenerateRandomKey();
        var uriPath = FindUriPath(url);

        var payload = request != null ? uriPath + JsonBuilder.SerializeObjectToPrettyJson(request) : uriPath;
        var dataToEncrypt = randomKey + payload;
        var hash = HashGeneratorV2.GenerateHash(options.ApiKey, options.SecretKey, randomKey, dataToEncrypt);
        return IYZIWS_V2_HEADER_NAME + hash;
    }

    private static string PrepareAuthorizationStringWithUrlParam(BaseRequestV2 request, string url, Options options)
    {
        var randomKey = GenerateRandomKey();
        var uriPath = FindUriPath(url);
        var dataToEncrypt = randomKey + uriPath;

        var hash = HashGeneratorV2.GenerateHash(options.ApiKey, options.SecretKey, randomKey, dataToEncrypt);
        return IYZIWS_V2_HEADER_NAME + hash;
    }

    private static string GenerateRandomKey()
    {
        return DateTime.Now.ToString("ddMMyyyyhhmmssffff");
    }

    private static string FindUriPath(string url)
    {
        var startIndex = url.IndexOf("/v2");
        var endIndex = url.IndexOf("?");
        var length = endIndex == -1 ? url.Length - startIndex : endIndex - startIndex;
        return url.Substring(startIndex, length);
    }
}