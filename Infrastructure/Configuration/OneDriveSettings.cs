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
}