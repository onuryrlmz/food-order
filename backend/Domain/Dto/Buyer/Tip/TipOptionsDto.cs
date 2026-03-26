namespace Domain.Dto.Buyer.Tip;

public class TipOptionsDto
{
    public List<short> PresetPercentages { get; set; }
    public decimal OrderTotal { get; set; }
    public List<decimal> PresetAmounts { get; set; }
}
