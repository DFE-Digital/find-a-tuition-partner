namespace Application.Common.Models.Enquiry;

public record EnquiryResponseBaseModel : EnquiryBaseModel
{
    public string TuitionPartnerName { get; set; } = string.Empty;

    public string? TuitionPartnerSeoUrl { get; set; }
    public string Token { get; set; } = string.Empty;

    public string LocalAuthorityDistrict { get; set; } = null!;
    public List<string> EnquiryKeyStageSubjects { get; set; } = null!;
    public string? KeyStageAndSubjectsText { get; set; } = string.Empty;

    public string? TuitionSettingText { get; set; } = string.Empty;

    public string EnquiryTuitionSetting { get; set; } = string.Empty;

    public string? TutoringLogisticsText { get; set; } = string.Empty;

    public string EnquiryTutoringLogistics { get; set; } = null!;
    public string? SENDRequirementsText { get; set; } = string.Empty;

    public string? EnquirySENDRequirements { get; set; }

    public string? EnquiryAdditionalInformation { get; set; }
    public string? AdditionalInformationText { get; set; } = string.Empty;
    public string SupportReferenceNumber { get; set; } = null!;
}