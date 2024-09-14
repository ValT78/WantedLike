using System;
using System.Security.Cryptography;
using System.Text;

public class RSAEncryptionHelper
{
    public static string Encrypt(string plainText, string publicKey)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedData;

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
            encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
        }

        return Convert.ToBase64String(encryptedData);
    }
}



public class RSAKeyGenerator
{
    public static void GenerateKeys(out string publicKey, out string privateKey)
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        }
    }
}
