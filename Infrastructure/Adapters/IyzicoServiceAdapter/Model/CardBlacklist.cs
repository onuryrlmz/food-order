using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CardBlacklist : IyzipayResource
{
    public string CardUserKey { get; set; }
    public string CardToken { get; set; }
    public string CardNumber { get; set; }
    public bool Blacklisted { get; set; }

    public static CardBlacklist Create(CreateCardBlacklistRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CardBlacklist>(options.BaseUrl + "/cardstorage/blacklist/card", GetHttpHeaders(request, options), request);
    }

    public static CardBlacklist Update(UpdateCardBlacklistRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CardBlacklist>(options.BaseUrl + "/cardstorage/blacklist/card/inactive", GetHttpHeaders(request, options), request);
    }

    public static CardBlacklist Retrieve(RetrieveCardBlacklistRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CardBlacklist>(options.BaseUrl + "/cardstorage/blacklist/card/retrieve", GetHttpHeaders(request, options), request);
    }
}