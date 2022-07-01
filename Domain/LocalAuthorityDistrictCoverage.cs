namespace Domain;

public class LocalAuthorityDistrictCoverage
{
    public int Id { get; set; }
    public int TuitionPartnerId { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public int TuitionTypeId { get; set; }
    public TuitionType TuitionType { get; set; } = null!;
    public int LocalAuthorityDistrictId { get; set; }
    public LocalAuthorityDistrict LocalAuthorityDistrict { get; set; } = null!;
}