namespace Application.Common.Models.Enquiry.Build;

public record TuitionPartnerMagicLinkModel
{
    public string TuitionPartnerSeoUrl { get; set; } = null!;

    public string MagicLinkToken { get; set; } = null!;

    public string Email { get; set; } = null!;

}