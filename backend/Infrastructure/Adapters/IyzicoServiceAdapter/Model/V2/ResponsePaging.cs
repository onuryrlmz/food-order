namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model.V2;

public class ResponsePaging<T>
{
    public List<T> Items { get; set; }
    public long? TotalCount { get; set; }
    public int? CurrentPage { get; set; }
    public int? PageCount { get; set; }
}
