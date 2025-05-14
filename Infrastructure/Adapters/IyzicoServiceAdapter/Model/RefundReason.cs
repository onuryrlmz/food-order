namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public sealed class RefundReason
{
    public static readonly RefundReason DOUBLE_PAYMENT = new("double_payment");
    public static readonly RefundReason BUYER_REQUEST = new("buyer_request");
    public static readonly RefundReason FRAUD = new("fraud");
    public static readonly RefundReason OTHER = new("other");
    private readonly string value;

    private RefundReason(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }
}