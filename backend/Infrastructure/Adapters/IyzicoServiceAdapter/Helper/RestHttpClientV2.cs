using System.Net;
using Newtonsoft.Json;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

internal class RestHttpClientV2
{
    private static readonly HttpClient HttpClient;

    static RestHttpClientV2()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        HttpClient = new HttpClient();
    }

    public static RestHttpClientV2 Create()
    {
        return new RestHttpClientV2();
    }

    public T Get<T>(string url, Dictionary<string, string> headers) where T : IyzipayResourceV2
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        var response = JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
        response.AppendWithHttpResponseHeaders(httpResponseMessage);
        return response;
    }

    public T Post<T>(string url, Dictionary<string, string> headers, BaseRequestV2 request) where T : IyzipayResourceV2
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        var response = JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
        response.AppendWithHttpResponseHeaders(httpResponseMessage);
        return response;
    }

    public T Put<T>(string url, Dictionary<string, string> headers, BaseRequestV2 request) where T : IyzipayResourceV2
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        var response = JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
        response.AppendWithHttpResponseHeaders(httpResponseMessage);
        return response;
    }

    public T Delete<T>(string url, Dictionary<string, string> headers, BaseRequestV2 request) where T : IyzipayResourceV2
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        var response = JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
        response.AppendWithHttpResponseHeaders(httpResponseMessage);
        return response;
    }
}