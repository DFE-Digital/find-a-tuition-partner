using Domain.Enums;

namespace Application.Common.Models.Enquiry.Build;

public class EnquirerEmailModel
{
    public EnquirerEmailModel()
    {

    }
    public EnquirerEmailModel(EnquirerEmailModel model)
    {
        Email = model.Email;
        From = model.From;
    }
    public string? Email { get; set; }
    public ReferrerList? From { get; set; }

    public string[]? Subjects { get; set; }

    public KeyStage[]? KeyStages { get; set; }
}