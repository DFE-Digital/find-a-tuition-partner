namespace Application.Common.Interfaces;

public interface IEncrypt
{
    string Encrypt(string plaintext);
    string Decrypt(string buffer);
}