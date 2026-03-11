using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class BkmInstallment : RequestStringConvertible
{
    public long? BankId { get; set; }
    public List<BkmInstallmentPrice> InstallmentPrices { get; set; }

    public string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .Append("bankId", BankId)
            .AppendList("installmentPrices", InstallmentPrices)
            .GetRequestString();
    }
}