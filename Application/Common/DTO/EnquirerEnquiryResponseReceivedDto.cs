using Application.Common.Models;

namespace Application.Common.DTO;

public record EnquirerEnquiryResponseReceivedDto : EnquiryResponseModel
{
    public string TuitionPartnerName { get; set; } = string.Empty;
}