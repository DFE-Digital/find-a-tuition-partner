namespace Infrastructure.Configuration;

public class DfeAnalyticsSettings
{
    public const string DfeAnalyticsConfigName = "DfeAnalytics";
    
    public string DatasetId { get; set; } = string.Empty;

    public string CredentialsJsonFile { get; set; } = string.Empty;

    public string ProjectId { get; set; } = string.Empty;
}