namespace Infrastructure.Configuration;

public class EmailSettings
{
    public const string EmailSettingsConfigName = "EmailSettings";

    public bool SendEmailsFromNtp { get; set; } = true;

    public string OverrideAddress { get; set; } = string.Empty;

    public bool MergeResponses { get; set; } = false;

    public bool AllSentToEnquirer { get; set; } = false;

    public bool SendTuitionPartnerEmailsWhenEnquirerDelivered { get; set; } = true;

}