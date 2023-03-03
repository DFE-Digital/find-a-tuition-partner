using Application.Common.Models.Enquiry.Build;

namespace Application.Common.Models.Enquiry.Respond;

public record EnquiryResponseModel : EnquiryBaseModel
{
    public int TuitionPartnerId { get; set; }
    public string TuitionPartnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string EnquiryText { get; set; } = string.Empty;
    public string EnquiryResponseText { get; set; } = null!;
}