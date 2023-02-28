using Domain.Search;

namespace Application.Common.Models.Enquiry.Build;

public record EnquiryBuildModel : EnquiryBaseModel
{
    public string? EnquiryText { get; set; }

    public string? Email { get; set; }

    public TuitionPartnersResult? TuitionPartnersForEnquiry { get; set; }
}