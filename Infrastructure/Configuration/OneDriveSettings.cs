namespace Infrastructure.Configuration;

public class OneDriveSettings
{
    public const string OneDrive = "OneDrive";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string SharedDriveId { get; set; } = string.Empty;
    public string TuitionPartnerDataFolderId { get; set; } = string.Empty;
    public string TuitionPartnerLogosFolderId { get; set; } = string.Empty;

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