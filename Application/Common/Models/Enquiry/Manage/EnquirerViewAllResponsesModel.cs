using Application.Common.DTO;

namespace Application.Common.Models.Enquiry.Manage;

public class EnquirerViewAllResponsesModel
{
    public EnquirerViewAllResponsesModel()
    {
        TutoringLogisticsDisplayModel = new TutoringLogisticsDisplayModel();
    }

    public string LocalAuthorityDistrict { get; set; } = null!;

    public TutoringLogisticsDisplayModel TutoringLogisticsDisplayModel { get; set; }

    public string? ErrorMessage { get; set; }

    public int NumberOfTpEnquiryWasSent { get; set; }

    public int NumberOfTpsDeclinedEnquiry { get; set; }

    public string SupportReferenceNumber { get; set; } = null!;

    public string TuitionSettingName { get; set; } = null!;
    public List<string> KeyStageSubjects { get; set; } = null!;

    public string? SENDRequirements { get; set; }

    public string? AdditionalInformation { get; set; }
    public DateTime EnquiryCreatedDateTime { get; set; }

    public string Token { get; set; } = string.Empty;

    public List<EnquirerViewResponseDto> EnquirerViewResponses { get; set; } = null!;
}