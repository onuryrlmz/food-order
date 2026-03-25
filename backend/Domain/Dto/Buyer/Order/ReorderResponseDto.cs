namespace Domain.Dto.Buyer.Order;

public class ReorderResponseDto
{
    public bool Success { get; set; }
    public List<ReorderWarningDto> Warnings { get; set; } = new();
}

public class ReorderWarningDto
{
    public Guid? MenuId { get; set; }
    public string MenuName { get; set; }
    public string WarningType { get; set; } // "Unavailable", "PriceChanged"
    public string Message { get; set; }
}