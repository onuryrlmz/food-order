namespace Domain.Dto.Buyer.Tip;

public class AddTipRequestDto
{
    public Guid OrderId { get; set; }
    public decimal? CustomAmount { get; set; }
    public short? PresetPercentage { get; set; }
}
