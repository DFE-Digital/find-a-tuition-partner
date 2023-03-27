namespace Application.Common.Models.Enquiry.Respond;

public record EnquiryResponseToTpPersonalisationInput
{
    public EnquiryResponseToTpPersonalisationInput()
    {

    }

    public string? TpName { get; init; }
    public string? SupportRefNumber { get; init; }
    public string? LocalAreaDistrict { get; init; }
    public string? CreatedOnDateTime { get; init; }
    public string? EnquiryKeyStageSubjects { get; init; }
    public string? EnquiryResponseKeyStageSubjects { get; init; }
    public string? EnquiryTuitionType { get; init; }
    public string? EnquiryResponseTuitionType { get; init; }
    public string? EnquiryTuitionPlan { get; init; }
    public string? EnquiryResponseTuitionPlan { get; init; }
    public string? EnquirySendSupport { get; init; }
    public string? EnquiryResponseSendSupport { get; init; }
    public string? EnquiryAdditionalInformation { get; init; }
    public string? EnquiryResponseAdditionalInformation { get; init; }
    public string? ContactUsLink { get; init; }
}
