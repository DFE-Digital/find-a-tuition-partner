namespace Application.Common.Models.Enquiry.Respond;

public record ViewAndCaptureEnquiryResponseModel : EnquiryResponseModel
{
    public string EnquiryResponseCloseDateFormatted { get; set; } = string.Empty;
}