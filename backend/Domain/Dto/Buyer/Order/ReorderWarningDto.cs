namespace Domain.Dto.Buyer.Order;

public class ReorderWarningDto
{
    public Guid? MenuId { get; set; }
    public string MenuName { get; set; }
    public string WarningType { get; set; } // "Unavailable", "PriceChanged"
    public string Message { get; set; }
}
