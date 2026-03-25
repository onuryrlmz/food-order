namespace Domain.Dto.Seller.Courier;

public class UpdateDeliverySettingsRequestDto
{
    public short? DefaultAssignmentStrategyId { get; set; }
    public bool HasOwnCouriers { get; set; }
}