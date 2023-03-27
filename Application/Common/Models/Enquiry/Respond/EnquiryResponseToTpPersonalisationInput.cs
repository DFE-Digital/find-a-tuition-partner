namespace Application.Common.Models.Enquiry.Respond;

public record EnquiryResponseToTpPersonalisationInput
{
    public EnquiryResponseToTpPersonalisationInput()
    {

    }
    public EnquiryResponseToTpPersonalisationInput(
        string tpName,
        string supportRefNumber,
        string localAreaDistrict,
        string responseCreatedOnDateTime,
        string enquiryKeyStageSubjects,
        string enquiryResponseKeyStageSubjects,
        string enquiryTuitionType,
        string enquiryResponseTuitionType,
        string enquiryTuitionPlan,
        string enquiryResponseTuitionPlan,
        string enquirySendSupport,
        string enquiryResponseSendSupport,
        string enquiryAdditionalInformation,
        string enquiryResponseAdditionalInformation,
        string contactUsLink)
    {
        TpName = tpName;
        SupportRefNumber = supportRefNumber;
        LocalAreaDistrict = localAreaDistrict;
        ResponseCreatedOnDateTime = responseCreatedOnDateTime;
        EnquiryKeyStageSubjects = enquiryKeyStageSubjects;
        EnquiryResponseKeyStageSubjects = enquiryResponseKeyStageSubjects;
        EnquiryTuitionType = enquiryTuitionType;
        EnquiryResponseTuitionType = enquiryResponseTuitionType;
        EnquiryTuitionPlan = enquiryTuitionPlan;
        EnquiryResponseTuitionPlan = enquiryResponseTuitionPlan;
        EnquirySendSupport = enquirySendSupport;
        EnquiryResponseSendSupport = enquiryResponseSendSupport;
        EnquiryAdditionalInformation = enquiryAdditionalInformation;
        EnquiryResponseAdditionalInformation = enquiryResponseAdditionalInformation;
        ContactUsLink = contactUsLink;
    }
    public string? TpName { get; init; }
    public string? SupportRefNumber { get; init; }
    public string? LocalAreaDistrict { get; init; }
    public string? ResponseCreatedOnDateTime { get; init; }
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
