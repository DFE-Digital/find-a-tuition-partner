using System.ComponentModel;

namespace Domain.Enums;

public enum EmailTemplateType
{
    [Description("esce")]
    EnquirySubmittedConfirmationToEnquirer,
    [Description("esnt")]
    EnquirySubmittedToTp,
    [Description("erne")]
    EnquiryResponseReceivedConfirmationToEnquirer,
    [Description("erct")]
    EnquiryResponseSubmittedConfirmationToTp,
    [Description("ever")]
    EmailVerification,
    [Description("eotp")]
    EnquiryOutcomeToTp,
    [Description("ede")]
    DataExtraction
}