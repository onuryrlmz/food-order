using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request;

public class DeleteCardRequest : BaseRequest
{
    public string CardUserKey { get; set; }
    public string CardToken { get; set; }

    public override string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .AppendSuper(base.ToPKIRequestString())
            .Append("cardUserKey", CardUserKey)
            .Append("cardToken", CardToken)
            .GetRequestString();
    }
}