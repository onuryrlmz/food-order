using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Base.Helpers;

public static class ExtHelper
{
    public static string GetDescriptionFromEnumValue(this object value)
    {
        if (value == null) return "";
        return !(value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() is DescriptionAttribute attribute)
            ? value.ToString()
            : attribute.Description;
    }

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +

            // Convert the byte array to hexadecimal string prior to .NET 5
            // StringBuilder sb = new System.Text.StringBuilder();
            // for (int i = 0; i < hashBytes.Length; i++)
            // {
            //     sb.Append(hashBytes[i].ToString("X2"));
            // }
            // return sb.ToString();
        }
    }
}