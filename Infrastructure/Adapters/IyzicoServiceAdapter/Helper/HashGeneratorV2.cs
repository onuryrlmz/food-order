using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Adapters.IyzicoServiceAdapter.Helper;

public class HashGeneratorV2
{
    private HashGeneratorV2()
    {
    }

    public static string GenerateHash(string apiKey, string secretKey, string randomString, string dataToEncrypt)
    {
        HashAlgorithm algorithm = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var computedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(dataToEncrypt));
        var computedHashAsHex = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
        var authorizationString = "apiKey:" + apiKey + "&randomKey:" + randomString + "&signature:" + computedHashAsHex;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(authorizationString));
    }
}