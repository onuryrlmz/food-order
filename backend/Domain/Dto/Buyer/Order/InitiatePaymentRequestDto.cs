namespace Domain.Dto.Buyer.Order;

public class InitiatePaymentRequestDto
{
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string ExpireMonth { get; set; }
    public string ExpireYear { get; set; }
    public string Cvc { get; set; }
    public string BuyerIp { get; set; } = "85.34.78.112";
}

public class InitiatePaymentResponseDto
{
    public bool RequiresThreeDs { get; set; }
    public string? ThreeDsHtmlContent { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
