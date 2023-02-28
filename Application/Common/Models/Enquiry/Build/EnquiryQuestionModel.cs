using Domain.Enums;

namespace Application.Common.Models.Enquiry.Build;

public class EnquiryQuestionModel
{
    public EnquiryQuestionModel()
    {

    }
    public EnquiryQuestionModel(EnquiryQuestionModel model)
    {
        EnquiryText = model.EnquiryText;
        From = model.From;
    }
    public string? EnquiryText { get; set; }
    public ReferrerList? From { get; set; }
}