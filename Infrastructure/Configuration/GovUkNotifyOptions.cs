namespace Infrastructure.Configuration;

public class GovUkNotifyOptions
{
    public const string GovUkNotify = "GovUkNotify";

    public string ApiKey { get; set; } = string.Empty;

    public string TemplateIdEnquirySubmittedConfirmationToEnquirer { get; set; } = string.Empty;

    public string TemplateIdEnquirySubmittedToTp { get; set; } = string.Empty;

    public string TemplateIdEnquiryResponseReceivedConfirmationToEnquirer { get; set; } = string.Empty;

    public string TemplateIdEnquiryResponseSubmittedConfirmationToTp { get; set; } = string.Empty;

}