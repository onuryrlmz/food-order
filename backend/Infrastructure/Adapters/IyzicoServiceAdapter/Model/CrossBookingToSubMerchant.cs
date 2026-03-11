using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CrossBookingToSubMerchant : IyzipayResource
{
    public static CrossBookingToSubMerchant Create(CreateCrossBookingRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CrossBookingToSubMerchant>(options.BaseUrl + "/crossbooking/send", GetHttpHeaders(request, options), request);
    }
}