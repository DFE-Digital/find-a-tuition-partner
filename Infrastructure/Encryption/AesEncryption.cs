using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Encryption;

public class AesEncryption : IEncrypt
{
    private readonly EncryptionOptions _config;

    public AesEncryption(IOptions<EncryptionOptions> options)
    {
        _config = options.Value;
    }

    public string Encrypt(string plaintext)
    {
        byte[] encryptedBytes;

        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_config.Key);
            aes.IV = Convert.FromBase64String(_config.IV);

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(
                    resultStream,
                    encryptor,
                    CryptoStreamMode.Write))
                {
                    aesStream.Write(Encoding.UTF8.GetBytes(plaintext), 0, Encoding.UTF8.GetByteCount(plaintext));
                }

                encryptedBytes = resultStream.ToArray();
            }
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string buffer)
    {
        byte[] encryptedBytes = Convert.FromBase64String(buffer);
        byte[] decryptedBytes;

        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_config.Key);
            aes.IV = Convert.FromBase64String(_config.IV);

            using (var decrypter = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(
                    resultStream,
                    decrypter,
                    CryptoStreamMode.Write))
                {
                    aesStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                }

                decryptedBytes = resultStream.ToArray();
            }
        }

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}