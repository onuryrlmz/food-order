namespace Domain.Dto.Buyer.Tip;

public class TipDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public short? PresetPercentage { get; set; }
    public bool IsPreDelivery { get; set; }
    public DateTime CreatedDate { get; set; }
}
