using System.Security.Cryptography;
using System.Text;

namespace Base.Security;

public static class CryptoManagerV1
{
    public static Encoding Encoding { get; set; }

    public static string Encrypt(string chipperText, string encryptionKey = "")
    {
        try
        {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            if (string.IsNullOrEmpty(encryptionKey))
                encryptionKey = "";
            if (string.IsNullOrEmpty(chipperText))
                return chipperText;
            var DES = TripleDES.Create();
            var hashMD5 = MD5.Create();
            DES.Key = hashMD5.ComputeHash(Encoding.GetBytes(encryptionKey));
            DES.Mode = CipherMode.ECB;
            var Encryptor = DES.CreateEncryptor();
            var Buffer = Encoding.GetBytes(chipperText);
            return Convert.ToBase64String(Encryptor.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        catch
        {
            return chipperText;
        }
    }

    public static string Decrypt(string richText, string decryptionKey = "")
    {
        try
        {
            if (Encoding == null)
                Encoding = Encoding.UTF8;
            if (string.IsNullOrEmpty(decryptionKey))
                decryptionKey = "";
            if (string.IsNullOrEmpty(richText))
                return richText;
            var DES = TripleDES.Create();
            var hashMD5 = MD5.Create();
            DES.Key = hashMD5.ComputeHash(Encoding.GetBytes(decryptionKey));
            DES.Mode = CipherMode.ECB;
            var Decryptor = DES.CreateDecryptor();
            var Buffer = Convert.FromBase64String(richText);
            return Encoding.GetString(Decryptor.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        catch
        {
            return richText;
        }
    }

    public static string ComputeHash(string plainText, string saltText = "", HashAlgorithms algorithm = HashAlgorithms.SHA256, Encoding encoding = null)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            if (encoding == null) encoding = Encoding.UTF8;
            HashAlgorithm hashProvider = null;
            switch (algorithm)
            {
                default:
                case HashAlgorithms.SHA1:
                    hashProvider = SHA1.Create();
                    break;
                case HashAlgorithms.SHA256:
                    hashProvider = SHA256.Create();
                    break;
                case HashAlgorithms.SHA384:
                    hashProvider = SHA384.Create();
                    break;
                case HashAlgorithms.SHA512:
                    hashProvider = SHA512.Create();
                    break;
                case HashAlgorithms.MD5:
                    hashProvider = MD5.Create();
                    break;
            }

            var hashedText = plainText + saltText;
            var hashbytes = encoding.GetBytes(hashedText);
            var inputbytes = hashProvider.ComputeHash(hashbytes);
            hashProvider.Clear();
            return CryptoHelper.GetHexaDecimal(inputbytes);
        }
        catch
        {
            return plainText;
        }
    }

    public static string GetMd5Hash(string plainText, string saltText = "")
    {
        return ComputeHash(plainText, saltText, HashAlgorithms.MD5);
    }

    public static string GetSHA512Hash(string plainText, string saltText = "")
    {
        return ComputeHash(plainText, saltText, HashAlgorithms.SHA512);
    }
}

public static class CryptoManagerV3
{
    // Token encryption için sabit IV/Salt — token veri güvenliği değil transport güvenliği sağlar.
    // Şifre hashing'i için BCrypt kullanılır (UserManager).
    private static readonly byte[] IvV2 = Convert.FromBase64String("uGLfDZHiv1NeHnQt+M68UQ==");
    private static readonly byte[] SaltV2 = Encoding.UTF8.GetBytes("Pu+FLO7Uuk+L8Mn");
    private const string PasswordV2 = "6ISemXimN+K!zAs";

    public static string Encrypt(this string strPlainText, string key = "")
    {
        using var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.IV = IvV2;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.Key = GenerateKey(PasswordV2, SaltV2, 1000);

        var strText = Encoding.UTF8.GetBytes(strPlainText);
        using var transform = aes.CreateEncryptor();
        var cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);
        return Convert.ToBase64String(cipherText);
    }

    public static string Decrypt(this string strChipperText, string key = "")
    {
        using var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.IV = IvV2;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.Key = GenerateKey(PasswordV2, SaltV2, 1000);

        var cipherBytes = Convert.FromBase64String(strChipperText);
        using var transform = aes.CreateDecryptor();
        var result = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(result);
    }

    private static byte[] GenerateKey(string strPassword, byte[] salt, int iterations)
    {
        using var rfc2898 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(strPassword), salt, iterations, HashAlgorithmName.SHA256);
        return rfc2898.GetBytes(128 / 8);
    }
}