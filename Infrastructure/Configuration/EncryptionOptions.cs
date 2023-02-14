namespace Infrastructure.Configuration;

public class EncryptionOptions
{
    public const string Encryption = "AesEncryption";
    public string Key { get; set; } = null!;
    public string IV { get; set; } = null!;
}