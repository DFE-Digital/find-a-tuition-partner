namespace Infrastructure.Configuration;

public class AzureBlobStorageSettings
{
    public const string AzureBlobStorage = "AzureBlobStorage";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string StorageAccountName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string TuitionPartnerDataFolderName { get; set; } = string.Empty;
    public string TuitionPartnerLogosFolderName { get; set; } = string.Empty;

    public void Validate()
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                throw new ArgumentException($"Property {property.Name} cannot be null or empty.");
            }
        }
    }
}