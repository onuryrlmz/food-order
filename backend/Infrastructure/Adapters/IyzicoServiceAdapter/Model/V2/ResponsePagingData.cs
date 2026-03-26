using Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2;

public class ResponsePagingData<T> : IyzipayResourceV2
{
    public ResponsePaging<T> Data { get; set; }
}
