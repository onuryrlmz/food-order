using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class CrossBookingFromSubMerchant : IyzipayResource
{
    public static CrossBookingFromSubMerchant Create(CreateCrossBookingRequest request, Options options)
    {
        return RestHttpClient.Create().Post<CrossBookingFromSubMerchant>(options.BaseUrl + "/crossbooking/receive", GetHttpHeaders(request, options), request);
    }
}