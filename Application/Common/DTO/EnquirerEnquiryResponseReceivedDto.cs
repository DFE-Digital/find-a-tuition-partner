using Application.Common.Models;

namespace Application.Common.DTO;

public class EnquirerEnquiryResponseReceivedDto : EnquiryResponseModel
{
    public string TuitionPartnerName { get; set; } = string.Empty;
}