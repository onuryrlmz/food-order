namespace Domain.Dto.Buyer.Search;

public class SearchHistoryDto
{
    public Guid Id { get; set; }
    public string Query { get; set; }
    public string? SearchType { get; set; }
    public int ResultCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
