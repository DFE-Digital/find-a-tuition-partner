using System;
using System.Security.Cryptography;
using Infrastructure.Configuration;
using Infrastructure.Encryption;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Infrastructure.Tests;

public class AesEncryptionTests
{
    private readonly EncryptionOptions _encryptionConfig;

    public AesEncryptionTests()
    {
        using var rng = RandomNumberGenerator.Create();
        var key = new byte[32]; // 256 bits
        var iv = new byte[16]; // 128 bits

        rng.GetBytes(key);
        rng.GetBytes(iv);

        _encryptionConfig = new EncryptionOptions
        {
            Key = Convert.ToBase64String(key),
            IV = Convert.ToBase64String(iv)
        };
    }

    [Fact]
    public void TestEncryptAndDecrypt()
    {
        // Arrange
        var mockOptions = new Mock<IOptions<EncryptionOptions>>();
        mockOptions.Setup(x => x.Value).Returns(_encryptionConfig);

        var aesEncryption = new AesEncryption(mockOptions.Object);
        var plaintext = "This is a secret message";

        // Act
        var encryptedText = aesEncryption.Encrypt(plaintext);
        var decryptedText = aesEncryption.Decrypt(encryptedText);

        // Assert
        Assert.NotEqual(plaintext, encryptedText);
        Assert.Equal(plaintext, decryptedText);
    }
}