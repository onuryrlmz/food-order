using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class RetrievePayWithIyzicoRequest : BaseRequest
{
    public string Token { set; get; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("token", Token)
            .GetRequestString();
    }
}