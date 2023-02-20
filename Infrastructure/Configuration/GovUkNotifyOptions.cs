namespace Infrastructure.Configuration;

public class GovUkNotifyOptions
{
    public const string GovUkNotify = "GovUkNotify";

    public string ApiKey { get; set; } = string.Empty;

    public string EnquiryTemplateId { get; set; } = string.Empty;

    public string EnquirerViewResponsesTemplateId { get; set; } = string.Empty;

    public string TestEmailAddress { get; set; } = string.Empty;

}