namespace Functions;

public class BlobStorageEnquiriesDataSettings
{
    public const string BlobStorageEnquiriesData = "BlobStorageEnquiriesData";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "ntp-enquiries-data-qa";

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