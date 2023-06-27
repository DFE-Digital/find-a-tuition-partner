namespace Application.Common.Models.Enquiry.Respond;

public record ViewAndCaptureEnquiryResponseModel : EnquiryResponseBaseModel
{
    public string EnquiryResponseCloseDateFormatted { get; set; } = string.Empty;

    public int NumberOfTpEnquiryWasSent { get; set; }
}