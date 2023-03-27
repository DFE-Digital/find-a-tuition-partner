namespace Application.Common.Models.Enquiry.Respond;

public record ViewAndCaptureEnquiryResponseModel : EnquiryResponseModel
{
    public DateTime EnquiryCreatedDateTime { get; set; }
}