namespace Domain.Dto.Seller.Courier;

public class CreateAgreementRequestDto
{
    public Guid? CourierCompanyId { get; set; }
    public Guid? CourierId { get; set; }
    public short AssignmentStrategyId { get; set; }
    public decimal? AgreedDeliveryFee { get; set; }
    public decimal? PerKmFee { get; set; }
    public int Priority { get; set; }
    public bool IsDefault { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveUntil { get; set; }
}