namespace Application.Common.Models.Enquiry.Build;

public record CheckYourAnswersModel : EnquiryBuildModel
{
    public Dictionary<string, List<string>>? KeyStageSubjects { get; set; }
}