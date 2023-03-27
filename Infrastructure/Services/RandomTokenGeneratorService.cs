using System.Security.Cryptography;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class RandomTokenGeneratorService : IRandomTokenGenerator
{
    public string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[25]; // 200 bits
        rng.GetBytes(bytes);

        // Truncate the last 4 bits to generate a 196-bit token
        bytes[24] &= 0xF0;

        // Convert the byte array to a Base64Url string
        var base64Url = Base64UrlEncode(bytes);

        return base64Url;
    }

    private string Base64UrlEncode(byte[] input)
    {
        var base64 = Convert.ToBase64String(input);
        // Replace characters that are not safe for use in URLs
        var base64Url = base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        return base64Url;
    }
}