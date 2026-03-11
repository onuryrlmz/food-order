using System.Text;

namespace Base.Security;

public static class CryptoHelper
{
    public static byte[] ConcatToByteArray(string password, string salt)
    {
        return Encoding.Unicode.GetBytes(string.Concat(salt, password));
    }

    public static string GetHexaDecimal(byte[] bytes)
    {
        var length = bytes.Length;
        var builder = new StringBuilder();
        for (var i = 0; i <= length - 1; i++)
            builder.Append(bytes[i].ToString("x2"));

        //for (int n = 0; n <= length - 1; n++)
        //    builder.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
        return builder.ToString();
    }
}