using System.ComponentModel;

namespace Domain.Enums;

public enum EmailTemplateType
{
    [Description("esce")]
    EnquirySubmittedConfirmationToEnquirer,
    [Description("estp")]
    EnquirySubmittedToTp,
    [Description("erne")]
    EnquiryResponseReceivedConfirmationToEnquirer,
    [Description("erct")]
    EnquiryResponseSubmittedConfirmationToTp
}