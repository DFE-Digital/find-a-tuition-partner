namespace Application.Common.Models.Enquiry.Build;

public record SubmittedConfirmationModel : SearchModel
{
    public SubmittedConfirmationModel() { TuitionPartnerMagicLinks = new Dictionary<string, string>(); }

    public SubmittedConfirmationModel(SearchModel model) : base(model) { TuitionPartnerMagicLinks = new Dictionary<string, string>(); }

    public string? SupportReferenceNumber { get; set; }

    public string? EnquirerEmailSentStatus { get; set; }
    public string? EnquirerMagicLink { get; set; }
    public Dictionary<string, string> TuitionPartnerMagicLinks { get; set; }
    public int? TuitionPartnerMagicLinksCount { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }
}