using Domain.Search;

namespace Application.Common.Models;

public record EnquiryModel : EnquiryBaseModel
{
    public string? EnquiryText { get; set; }

    public string? Email { get; set; }

    public TuitionPartnersResult? TuitionPartnersForEnquiry { get; set; }
}