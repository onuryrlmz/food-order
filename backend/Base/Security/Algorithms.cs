namespace Base.Security;

public enum SymmetricAlgorithms
{
    AES = 0,
    DES = 1,
    RC2 = 2,
    Rijndael = 3,
    TripleDES = 4
}

public enum AsymmetricAlgorithms
{
    DSA = 0,
    RSA = 1,
    ECDSA = 2,
    ECDiffieHellman = 3
}

public enum HashAlgorithms
{
    MD5 = 0,
    RIPEMD160 = 1,
    SHA1 = 2,
    SHA256 = 3,
    SHA384 = 4,
    SHA512 = 5
}