namespace Infrastructure.Adapters.IyzicoServiceAdapter.Request.V2.Subscription;

public class CardInfo
{
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string ExpireYear { get; set; }
    public string ExpireMonth { get; set; }
    public string Cvc { get; set; }
    public bool RegisterConsumerCard { get; set; }
    public string UcsToken { get; set; }
    public string CardToken { get; set; }
    public string ConsumerToken { get; set; }
}
