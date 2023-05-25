using Domain.Enums;

namespace Application.Common.Models.Enquiry.Build;

public record CheckYourAnswersModel : EnquiryBuildModel
{
    public Dictionary<KeyStage, List<Subject>>? KeyStageSubjects { get; set; }
    public bool HasKeyStageSubjects { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }
    public string? SchoolDetails { get; set; }
    public bool ConfirmTermsAndConditions { get; set; }
}