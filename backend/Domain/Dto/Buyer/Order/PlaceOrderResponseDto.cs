namespace Domain.Dto.Buyer.Order;

public class PlaceOrderResponseDto
{
    public Guid OrderId { get; set; }
    public bool RequiresPayment { get; set; }
    public bool RequiresThreeDs { get; set; }
    public string? ThreeDsHtmlContent { get; set; }
    public string? PaymentError { get; set; }
}
