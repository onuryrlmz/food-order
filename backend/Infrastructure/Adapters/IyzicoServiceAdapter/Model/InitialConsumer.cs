using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public class InitialConsumer
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string GsmNumber { get; set; }
    public List<IyziupAddress> AddressList { get; set; }

    public string ToPKIRequestString()
    {
        return ToStringRequestBuilder.NewInstance()
            .Append("name", Name)
            .Append("surname", Surname)
            .Append("email", Email)
            .Append("gsmNumber", GsmNumber)
            .Append("addressList", AddressList)
            .GetRequestString();
    }
}