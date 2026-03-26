namespace Domain.Dto.Buyer.Order;

public class InitiatePaymentResponseDto
{
    public bool RequiresThreeDs { get; set; }
    public string? ThreeDsHtmlContent { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
