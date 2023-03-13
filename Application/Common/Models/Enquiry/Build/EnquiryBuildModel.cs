using Domain.Search;

namespace Application.Common.Models.Enquiry.Build;

public record EnquiryBuildModel : EnquiryBaseModel
{
    public string? Email { get; set; }
    public string? TutoringLogistics { get; set; }
    public string? SENDRequirements { get; set; }
    public string? AdditionalInformation { get; set; }

    public TuitionPartnersResult? TuitionPartnersForEnquiry { get; set; }
}