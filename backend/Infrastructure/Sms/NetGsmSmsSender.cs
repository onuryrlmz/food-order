using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Infrastructure.Sms;

public class NetGsmSmsSender : ISmsSender
{
    private readonly IConfiguration _configuration;

    public NetGsmSmsSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendAsync(string phone, string message)
    {
        var userCode = _configuration["NetGsm:UserCode"];
        var password = _configuration["NetGsm:Password"];
        var header = _configuration["NetGsm:Header"];
        var baseUrl = _configuration["NetGsm:BaseUrl"] ?? "https://api.netgsm.com.tr";

        if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(password))
            return false;

        try
        {
            using var client = new RestClient(baseUrl);
            var request = new RestRequest("/sms/send/get", Method.Post);
            request.AddParameter("usercode", userCode);
            request.AddParameter("password", password);
            request.AddParameter("gsmno", phone);
            request.AddParameter("message", message);
            request.AddParameter("header", header ?? "RESTORAN");
            request.AddParameter("msgheader", header ?? "RESTORAN");
            request.AddParameter("dil", "TR");
            request.AddParameter("encoding", "tr");

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch
        {
            return false;
        }
    }
}
