namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        Coverage = new List<TuitionPartnerCoverage>();
        LocalAuthorityDistrictCoverage = new List<LocalAuthorityDistrictCoverage>();
        SubjectCoverage = new List<SubjectCoverage>();
        Prices = new List<Price>();
    }

    public int Id { get; set; }
    public DateOnly LastUpdated { get; set; }
    public string Name { get; set; } = null!;
    public string SeoUrl { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string LegalStatus { get; set; } = string.Empty;
    public bool HasSenProvision { get; set; }
    public string AdditionalServiceOfferings { get; set; } = string.Empty;

    public ICollection<TuitionPartnerCoverage> Coverage { get; set; }
    public ICollection<LocalAuthorityDistrictCoverage> LocalAuthorityDistrictCoverage { get; set; }
    public ICollection<SubjectCoverage> SubjectCoverage { get; set; }
    public ICollection<Price> Prices { get; set; }
}