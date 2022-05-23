namespace Domain;

public class TuitionPartnerCoverage
{
    public int Id { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public LocalAuthorityDistrict LocalAuthorityDistrict { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public TuitionType TuitionType { get; set; } = null!;
}