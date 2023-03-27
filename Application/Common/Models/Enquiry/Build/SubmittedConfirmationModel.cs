namespace Application.Common.Models.Enquiry.Build;

public record SubmittedConfirmationModel : SearchModel
{
    public SubmittedConfirmationModel() { TuitionPartnerMagicLinks = new List<TuitionPartnerMagicLinkModel>(); }

    public SubmittedConfirmationModel(SearchModel model) : base(model) { TuitionPartnerMagicLinks = new List<TuitionPartnerMagicLinkModel>(); }

    public string? SupportReferenceNumber { get; set; }

    public string? EnquirerEmailSentStatus { get; set; }
    public string? EnquirerMagicLink { get; set; }
    public List<TuitionPartnerMagicLinkModel> TuitionPartnerMagicLinks { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }

    public string ErrorStatus { get; set; } = string.Empty;
    public bool IsValid { get; set; } = true;
}