namespace Application.Common.Models.Enquiry.Build;

public class CheckYourAnswersModel : EnquiryBuildModel
{
    public Dictionary<string, List<string>>? KeyStageSubjects { get; set; }
}