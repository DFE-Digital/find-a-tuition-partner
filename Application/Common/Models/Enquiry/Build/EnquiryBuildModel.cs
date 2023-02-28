using Domain.Enums;

namespace Application.Common.Models.Enquiry.Build;

public class EnquiryBuildModel : EnquiryBaseModel
{
    public EnquiryBuildModel()
    {

    }

    public EnquiryBuildModel(EnquiryBuildModel model)
    {
        From = model.From;
        EnquiryText = model.EnquiryText;
        Email = model.Email;
        Postcode = model.Postcode;
        Subjects = model.Subjects;
        TuitionType = model.TuitionType;
        KeyStages = model.KeyStages;
    }

    public ReferrerList? From { get; set; }
    public string? EnquiryText { get; set; }
    public string? Email { get; set; }
    public List<string>? SelectedTuitionPartners { get; set; }

    public string? Postcode { get; set; }

    public string[]? Subjects { get; set; }

    public TuitionType? TuitionType { get; set; }

    public KeyStage[]? KeyStages { get; set; }
}