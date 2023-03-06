using Application.Common.DTO;

namespace Application.Common.Models.Enquiry.Respond;

public class EnquirerViewAllResponsesModel
{
    public string EnquiryText { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public int NumberOfTpEnquiryWasSent { get; set; }

    public List<EnquirerViewResponseDto> EnquirerViewResponses { get; set; } = null!;
}