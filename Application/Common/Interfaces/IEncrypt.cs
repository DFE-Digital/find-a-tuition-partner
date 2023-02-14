namespace Application.Common.Interfaces;

public interface IEncrypt
{
    string GenerateRandomToken();
    string Encrypt(string plaintext);
    string Decrypt(string buffer);
}