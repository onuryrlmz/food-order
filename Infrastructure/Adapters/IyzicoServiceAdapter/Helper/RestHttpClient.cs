using System.Net;
using Newtonsoft.Json;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

public class RestHttpClient
{
    private static readonly HttpClient HttpClient;

    static RestHttpClient()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        HttpClient = new HttpClient();
    }

    public static RestHttpClient Create()
    {
        return new RestHttpClient();
    }

    public T Get<T>(string url)
    {
        var httpResponseMessage = HttpClient.GetAsync(url).Result;
        return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
    }

    public T Get<T>(string url, Dictionary<string, string> headers)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
    }

    public T Post<T>(string url, Dictionary<string, string> headers, BaseRequest request)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
    }

    public T Delete<T>(string url, Dictionary<string, string> headers, BaseRequest request)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
    }

    public T Put<T>(string url, Dictionary<string, string> headers, BaseRequest request)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = JsonBuilder.ToJsonString(request)
        };

        foreach (var header in headers) requestMessage.Headers.Add(header.Key, header.Value);

        var httpResponseMessage = HttpClient.SendAsync(requestMessage).Result;
        return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
    }
}