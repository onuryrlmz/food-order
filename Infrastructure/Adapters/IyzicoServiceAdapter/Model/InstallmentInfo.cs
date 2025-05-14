using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;
using Infrastructure.Adapters.IyzicoServiceAdapter.Request;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class InstallmentInfo : IyzipayResource
{
    public List<InstallmentDetail> InstallmentDetails { get; set; }

    public static InstallmentInfo Retrieve(RetrieveInstallmentInfoRequest request, Options options)
    {
        return RestHttpClient.Create().Post<InstallmentInfo>(options.BaseUrl + "/payment/iyzipos/installment", GetHttpHeaders(request, options), request);
    }
}