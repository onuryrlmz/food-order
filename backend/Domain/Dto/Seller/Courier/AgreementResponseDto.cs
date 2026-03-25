namespace Domain.Dto.Seller.Courier;

public class AgreementResponseDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? CourierCompanyId { get; set; }
    public string? CourierCompanyName { get; set; }
    public Guid? CourierId { get; set; }
    public string? CourierName { get; set; }
    public short StatusId { get; set; }
    public short AssignmentStrategyId { get; set; }
    public decimal? AgreedDeliveryFee { get; set; }
    public decimal? PerKmFee { get; set; }
    public int Priority { get; set; }
    public bool IsDefault { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveUntil { get; set; }
    public DateTime CreatedDate { get; set; }
}