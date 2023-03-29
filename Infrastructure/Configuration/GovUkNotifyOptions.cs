namespace Infrastructure.Configuration;

public class GovUkNotifyOptions
{
    public const string GovUkNotify = "GovUkNotify";

    public string ApiKey { get; set; } = string.Empty;

    public string TemplateIdEnquirySubmittedConfirmationToEnquirer { get; set; } = "b1e29214-dfc0-4225-b65a-df6a20c4c297";

    public string TemplateIdEnquirySubmittedToTp { get; set; } = "b24ffca7-6119-4dbf-8ae2-dceb2c300ca4";

    public string TemplateIdEnquiryResponseReceivedConfirmationToEnquirer { get; set; } = "f64db52d-08ce-4f2a-9b37-a374db1ad77a";

    public string TemplateIdEnquiryResponseSubmittedConfirmationToTp { get; set; } = "0aad4d76-baad-456b-a58d-8221812e514c";

}