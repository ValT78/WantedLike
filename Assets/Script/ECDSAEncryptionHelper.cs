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

    public static string Decrypt(string cipherText, string privateKey)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(cipherText);
        byte[] decryptedData;

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
        }

        return Encoding.UTF8.GetString(decryptedData);
    }
}
