namespace Domain.Dto.Payment;

public class IyzicoPaymentRequestDto
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string BuyerEmail { get; set; }
    public string BuyerName { get; set; }
    public string BuyerSurname { get; set; }
    public string BuyerPhone { get; set; }
    public string BuyerIp { get; set; }
    public string BuyerId { get; set; }
    public string DeliveryCity { get; set; }
    public string DeliveryAddress { get; set; }
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string ExpireMonth { get; set; }
    public string ExpireYear { get; set; }
    public string Cvc { get; set; }
    public string CallbackUrl { get; set; }
}
