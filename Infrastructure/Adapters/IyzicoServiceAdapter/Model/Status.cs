namespace Infrastructure.Adapters.IyzicoServiceAdapter.Model;

public sealed class Status
{
    public static readonly Status SUCCESS = new("success");
    public static readonly Status FAILURE = new("failure");
    private readonly string value;

    private Status(string value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return value;
    }
}