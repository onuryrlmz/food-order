namespace Infrastructure.Adapters.IyzicoServiceAdapter;

public class CardDetailDto
{
    public string CardToken { get; set; }
    public string CardAlias { get; set; }
    public string BinNumber { get; set; }
    public string LastFourDigits { get; set; }
    public string CardType { get; set; }
    public string CardAssociation { get; set; }
    public string CardFamily { get; set; }
    public string CardBankName { get; set; }
    public long? CardBankCode { get; set; }
    public string ExpireMonth { get; set; }
    public string ExpireYear { get; set; }
}
