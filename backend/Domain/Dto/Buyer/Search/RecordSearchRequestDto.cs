namespace Domain.Dto.Buyer.Search;

public class RecordSearchRequestDto
{
    public string Query { get; set; }
    public string? SearchType { get; set; }
    public int ResultCount { get; set; }
}
