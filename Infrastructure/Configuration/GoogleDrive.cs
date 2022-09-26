namespace Infrastructure.Configuration;

public class GoogleDrive
{
    public string ServiceAccountKey { get; set; } = string.Empty;
    public string ServiceAccountKeyFilePath { get; set; } = string.Empty;
    public string SharedDriveId { get; set; } = string.Empty;
    public string TuitionPartnerDataFolderId { get; set; } = string.Empty;
    public string TuitionPartnerLogosFolderId { get; set; } = string.Empty;
}