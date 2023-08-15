namespace Infrastructure.Configuration;

public class GovUkNotifyOptions
{
    public const string GovUkNotify = "GovUkNotify";

    public string ApiKey { get; set; } = string.Empty;

    public string TemplateIdEnquirySubmittedConfirmationToEnquirer { get; set; } = "b1e29214-dfc0-4225-b65a-df6a20c4c297";

    public string TemplateIdEnquirySubmittedToTp { get; set; } = "b24ffca7-6119-4dbf-8ae2-dceb2c300ca4";

    public string TemplateIdEnquiryResponseReceivedConfirmationToEnquirer { get; set; } = "af83c1e0-1577-4f90-80e5-27035301ead9";

    public string TemplateIdEnquiryResponseSubmittedConfirmationToTp { get; set; } = "011fabe9-72b3-4575-a343-fe008ad4df90";

    public string TemplateIdEmailVerification { get; set; } = "badaacff-87b8-4046-b953-a85a0a9e2a8b";

    public string TemplateIdEnquiryOutcomeToTp { get; set; } = "4cc549a0-0295-4a9f-a9ab-91a1ddc572d0";

    public string TemplateIdDataExtraction { get; set; } = "cfb7c588-5acb-4e3b-8795-f46c5ee09633";

}