namespace Domain.Dto.Seller.Settlement;

public class SettlementItemDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal FixedFee { get; set; }
    public decimal NetAmount { get; set; }
    public short CommissionSourceType { get; set; }
    public DateTime CreatedDate { get; set; }
}
