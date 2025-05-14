namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public sealed class Locale
{
    public static readonly Locale EN = new("en");
    public static readonly Locale TR = new("tr");
    private readonly string value;

    private Locale(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }
}