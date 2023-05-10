namespace Application.Common.Models.Enquiry.Build;

public record SubmittedConfirmationModel : SearchModel
{
    public SubmittedConfirmationModel() { TuitionPartnerMagicLinks = new List<TuitionPartnerMagicLinkModel>(); }

    public SubmittedConfirmationModel(SearchModel model) : base(model) { TuitionPartnerMagicLinks = new List<TuitionPartnerMagicLinkModel>(); }

    public string? SupportReferenceNumber { get; set; }

    public string? EnquirerMagicLink { get; set; }
    public List<TuitionPartnerMagicLinkModel> TuitionPartnerMagicLinks { get; set; }

    public int TuitionPartnerMagicLinksCount { get; set; }
    public string? LocalAuthorityDistrictName { get; set; }

    public bool HadEmailSendException { get; set; } = false;
}