namespace Infrastructure.Configuration;

public class GovUkNotifyOptions
{
    public const string GovUkNotify = "GovUkNotify";

    public string ApiKey { get; set; } = string.Empty;

    public string TemplateIdEnquirySubmittedConfirmationToEnquirer { get; set; } = "b1e29214-dfc0-4225-b65a-df6a20c4c297";

    public string TemplateIdEnquirySubmittedToTp { get; set; } = "b24ffca7-6119-4dbf-8ae2-dceb2c300ca4";

    public string TemplateIdEnquiryResponseReceivedConfirmationToEnquirer { get; set; } = "f64db52d-08ce-4f2a-9b37-a374db1ad77a";

    public string TemplateIdEnquiryResponseSubmittedConfirmationToTp { get; set; } = "fa4f3699-4e98-4cea-add3-ac20ff05a0ca";

    public string TemplateIdEmailVerification { get; set; } = "badaacff-87b8-4046-b953-a85a0a9e2a8b";

}