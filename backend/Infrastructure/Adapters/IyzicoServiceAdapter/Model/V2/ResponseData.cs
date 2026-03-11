using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2;

public class ResponseData<T> : IyzipayResourceV2
{
    public T Data { get; set; }
}