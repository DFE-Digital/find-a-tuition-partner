namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        LocalAuthorityDistrictCoverage = new List<LocalAuthorityDistrictCoverage>();
        SubjectCoverage = new List<SubjectCoverage>();
        Prices = new List<Price>();
    }

    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public DateTime TPLastUpdatedData { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int OrganisationTypeId { get; set; }
    public OrganisationType OrganisationType { get; set; } = null!;
    public bool IsVatCharged { get; set; }
    public TuitionPartnerLogo? Logo { get; set; } = null!;
    public bool HasLogo { get; set; }

    public ICollection<LocalAuthorityDistrictCoverage> LocalAuthorityDistrictCoverage { get; set; }
    public ICollection<SubjectCoverage> SubjectCoverage { get; set; }
    public ICollection<Price> Prices { get; set; }
}