namespace Domain;

public class TuitionPartnerCoverage
{
    public int Id { get; set; }
    public int TuitionPartnerId { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public int LocalAuthorityDistrictId { get; set; }
    public LocalAuthorityDistrict LocalAuthorityDistrict { get; set; } = null!;
    public bool PrimaryLiteracy { get; set; }
    public bool PrimaryNumeracy { get; set; }
    public bool PrimaryScience { get; set; }
    public bool SecondaryEnglish { get; set; }
    public bool SecondaryHumanities { get; set; }
    public bool SecondaryMaths { get; set; }
    public bool SecondaryModernForeignLanguages { get; set; }
    public bool SecondaryScience { get; set; }
    public bool Online { get; set; }
    public bool InPerson { get; set; }
}