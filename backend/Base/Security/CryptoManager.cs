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
    // Token'lar authenticated encryption ile korunur: her şifrelemede rastgele IV + AES-256-CBC,
    // ardından (IV || ciphertext) üzerinde HMAC-SHA256. Gizli anahtar KOD'da tutulmaz; ortam
    // değişkeninden (TOKEN_SECURITY_KEY / TokenOptions:SecurityKey) türetilir. İmza doğrulaması
    // olmadan token forge edilemez; sabit IV/anahtar zafiyeti giderilir.
    private static readonly byte[] AesKeySalt = Encoding.UTF8.GetBytes("foodorder-token-aes-v3");
    private static readonly byte[] HmacKeySalt = Encoding.UTF8.GetBytes("foodorder-token-hmac-v3");
    private const int Pbkdf2Iterations = 10000;
    private const int IvLength = 16;
    private const int MacLength = 32;

    // Yapılandırma yoksa (ör. bazı test ortamları) süreç ömrü boyunca sabit rastgele anahtar
    // kullanılır — güvenli, ancak yeniden başlatmada token'lar geçersiz olur.
    private static readonly string EphemeralSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    // Anahtar türetme (PBKDF2) pahalıdır; süreç başına bir kez hesaplanıp önbelleğe alınır.
    private static readonly Lazy<(byte[] aesKey, byte[] hmacKey)> Keys = new(DeriveKeys);

    private static string ResolveSecret()
    {
        var secret = Base.Constant.Global.Configuration?["TokenOptions:SecurityKey"]
                     ?? Environment.GetEnvironmentVariable("TOKEN_SECURITY_KEY");
        return string.IsNullOrWhiteSpace(secret) ? EphemeralSecret : secret;
    }

    private static (byte[] aesKey, byte[] hmacKey) DeriveKeys()
    {
        var secretBytes = Encoding.UTF8.GetBytes(ResolveSecret());
        using var aesKdf = new Rfc2898DeriveBytes(secretBytes, AesKeySalt, Pbkdf2Iterations, HashAlgorithmName.SHA256);
        using var hmacKdf = new Rfc2898DeriveBytes(secretBytes, HmacKeySalt, Pbkdf2Iterations, HashAlgorithmName.SHA256);
        return (aesKdf.GetBytes(32), hmacKdf.GetBytes(32));
    }

    public static string Encrypt(this string strPlainText, string key = "")
    {
        var (aesKey, hmacKey) = Keys.Value;

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.Key = aesKey;
        aes.GenerateIV();
        var iv = aes.IV;

        var plainBytes = Encoding.UTF8.GetBytes(strPlainText);
        using var transform = aes.CreateEncryptor();
        var cipher = transform.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var ivAndCipher = new byte[iv.Length + cipher.Length];
        Buffer.BlockCopy(iv, 0, ivAndCipher, 0, iv.Length);
        Buffer.BlockCopy(cipher, 0, ivAndCipher, iv.Length, cipher.Length);

        using var hmac = new HMACSHA256(hmacKey);
        var mac = hmac.ComputeHash(ivAndCipher);

        var output = new byte[ivAndCipher.Length + mac.Length];
        Buffer.BlockCopy(ivAndCipher, 0, output, 0, ivAndCipher.Length);
        Buffer.BlockCopy(mac, 0, output, ivAndCipher.Length, mac.Length);
        return Convert.ToBase64String(output);
    }

    public static string Decrypt(this string strChipperText, string key = "")
    {
        try
        {
            var (aesKey, hmacKey) = Keys.Value;
            var data = Convert.FromBase64String(strChipperText);
            if (data.Length < IvLength + MacLength + 1)
                return string.Empty;

            var macOffset = data.Length - MacLength;
            var ivAndCipher = new byte[macOffset];
            Buffer.BlockCopy(data, 0, ivAndCipher, 0, macOffset);
            var providedMac = new byte[MacLength];
            Buffer.BlockCopy(data, macOffset, providedMac, 0, MacLength);

            using var hmac = new HMACSHA256(hmacKey);
            var expectedMac = hmac.ComputeHash(ivAndCipher);

            // İmza geçersizse (forge/tamper) sabit zamanlı karşılaştırma ile reddet.
            if (!CryptographicOperations.FixedTimeEquals(providedMac, expectedMac))
                return string.Empty;

            var iv = new byte[IvLength];
            Buffer.BlockCopy(ivAndCipher, 0, iv, 0, IvLength);
            var cipher = new byte[ivAndCipher.Length - IvLength];
            Buffer.BlockCopy(ivAndCipher, IvLength, cipher, 0, cipher.Length);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = aesKey;
            aes.IV = iv;
            using var transform = aes.CreateDecryptor();
            var result = transform.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(result);
        }
        catch
        {
            return string.Empty;
        }
    }
}