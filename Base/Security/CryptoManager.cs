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
    private const string IvV2 = "uGLfDZHiv1NeHnQt+M68UQ==";
    private const string SaltV2 = "Pu+FLO7Uuk+L8Mn";
    private const string PasswordV2 = "6ISemXimN+K!zAs";

    public static string Encrypt(this string strPlainText, string key = "")
    {
        using (var myRijndael = new RijndaelManaged())
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = Convert.FromBase64String(IvV2);
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Key = GenerateKey(PasswordV2, Encoding.UTF8.GetBytes(SaltV2), 1000);
            byte[] strText = Encoding.UTF8.GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);
            return Convert.ToBase64String(cipherText);
        }
    }

    public static string Decrypt(this string strChipperText, string key = "")
    {
        using (var myRijndael = new RijndaelManaged())
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = Convert.FromBase64String(IvV2);
            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Key = GenerateKey(PasswordV2, Encoding.UTF8.GetBytes(SaltV2), 1000);
            byte[] strChipperTextByte = Convert.FromBase64String(strChipperText);
            ICryptoTransform transform = myRijndael.CreateDecryptor();
            byte[] resultArray = transform.TransformFinalBlock(strChipperTextByte, 0, strChipperTextByte.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }

    private static byte[] HexStringToByteArray(string strHex)
    {
        dynamic r = new byte[strHex.Length / 2];
        for (var i = 0; i <= strHex.Length - 1; i += 2)
        {
            r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
        }

        return r;
    }

    private static byte[] GenerateKey(string strPassword, byte[] salt, int iterations)
    {
        Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(strPassword), salt, iterations);
        return rfc2898.GetBytes(128 / 8);
    }
}