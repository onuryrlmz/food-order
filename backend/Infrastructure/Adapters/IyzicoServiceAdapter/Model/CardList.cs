using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CardList : IyzipayResource
{
    public string CardUserKey { get; set; }
    public List<Card> CardDetails { get; set; }

    public static CardList Retrieve(RetrieveCardListRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CardList>(options.BaseUrl + "/cardstorage/cards", GetHttpHeaders(request, options), request);
    }
}