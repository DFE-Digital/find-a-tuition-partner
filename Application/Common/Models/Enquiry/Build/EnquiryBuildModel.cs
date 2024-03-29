using Domain.Search;

namespace Application.Common.Models.Enquiry.Build;

public record EnquiryBuildModel : EnquiryBaseModel
{
    public EnquiryBuildModel()
    {
        TutoringLogisticsDetailsModel = new TutoringLogisticsDetailsModel();
    }

    public int? SchoolId { get; set; }
    public string? Email { get; set; }
    public TutoringLogisticsDetailsModel TutoringLogisticsDetailsModel { get; set; }
    public string? SENDRequirements { get; set; }
    public string? AdditionalInformation { get; set; }

    public TuitionPartnersResult? TuitionPartnersForEnquiry { get; set; }
}