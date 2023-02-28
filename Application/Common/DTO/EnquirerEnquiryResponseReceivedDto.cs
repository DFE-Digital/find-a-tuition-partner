using Application.Common.Models.Enquiry.Respond;

namespace Application.Common.DTO;

public record EnquirerEnquiryResponseReceivedDto : EnquiryResponseModel
{
    public string TuitionPartnerName { get; set; } = string.Empty;
}