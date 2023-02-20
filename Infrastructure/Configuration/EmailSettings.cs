namespace Infrastructure.Configuration;

public class EmailSettings
{
    public const string EmailSettingsConfigName = "EmailSettings";

    public string OverrideAddress { get; set; } = string.Empty;

    public bool AmalgamateResponses { get; set; } = false;

    public bool AllSentToEnquirer { get; set; } = false;

}