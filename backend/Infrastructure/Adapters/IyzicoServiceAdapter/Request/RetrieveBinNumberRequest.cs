using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class RetrieveBinNumberRequest : BaseRequest
{
    public string BinNumber { get; set; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("binNumber", BinNumber)
            .GetRequestString();
    }
}