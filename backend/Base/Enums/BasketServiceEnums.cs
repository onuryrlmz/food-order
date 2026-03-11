namespace Base.Enums;

public static class BasketServiceEnums
{
    public enum BasketStatusEnums : short
    {
        Waiting = 1,
        PreparedForSale = 2,
        Canceled = 3,
        CanceledForSystem = 4,
        Completed = 5
    }

    public enum PaymentOptionEnums : short
    {
        CreditCard = 1,
        Wallet = 2
    }
}