using Application.Common.Models.Enquiry.Build;

namespace Application.Common.Models.Enquiry.Respond;

public record EnquiryResponseModel : EnquiryBaseModel
{
    public int TuitionPartnerId { get; set; }
    public string TuitionPartnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public string LocalAuthorityDistrict { get; set; } = null!;
    public List<string> EnquiryKeyStageSubjects { get; set; } = null!;
    public string? KeyStageAndSubjectsText { get; set; } = string.Empty;

    public string? TuitionTypeText { get; set; } = string.Empty;

    public string EnquiryTuitionType { get; set; } = string.Empty;

    public string? TutoringLogisticsText { get; set; } = string.Empty;

    public string EnquiryTutoringLogistics { get; set; } = null!;
    public string? SENDRequirementsText { get; set; } = string.Empty;

    public string? EnquirySENDRequirements { get; set; }

    public string? EnquiryAdditionalInformation { get; set; }
    public string? AdditionalInformationText { get; set; } = string.Empty;
}