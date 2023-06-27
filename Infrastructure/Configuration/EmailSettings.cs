namespace Infrastructure.Configuration;

public class EmailSettings
{
    public const string EmailSettingsConfigName = "EmailSettings";

    public string OverrideAddress { get; set; } = string.Empty;

    public bool MergeResponses { get; set; } = false;

    public bool AllSentToEnquirer { get; set; } = false;

    public int MinsDelaySendingOutcomeEmailToTP { get; set; } = 1440; //1440 = 24 hours * 60 mins
}