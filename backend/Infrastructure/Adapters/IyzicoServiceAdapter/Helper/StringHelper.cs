namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

internal class StringHelper
{
    public static string Reverse(string s)
    {
        var charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}