using Application.Common.DTO;

namespace Application.Common.Models.Enquiry.Respond;

public class EnquirerViewAllResponsesModel
{
    public string TutoringLogistics { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public int NumberOfTpEnquiryWasSent { get; set; }

    public string SupportReferenceNumber { get; set; } = null!;

    public string TuitionTypeName { get; set; } = null!;
    public List<string> KeyStageSubjects { get; set; } = null!;
    public DateTime EnquiryCreatedDateTime { get; set; }

    public List<EnquirerViewResponseDto> EnquirerViewResponses { get; set; } = null!;
}