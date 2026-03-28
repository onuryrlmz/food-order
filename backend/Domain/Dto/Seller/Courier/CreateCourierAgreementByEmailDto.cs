namespace Domain.Dto.Seller.Courier;

public class CreateCourierAgreementByEmailDto
{
    public Guid RestaurantId { get; set; }
    public string CourierEmail { get; set; } = string.Empty;
    public decimal? AgreedDeliveryFee { get; set; }
    public decimal? PerKmFee { get; set; }
}
