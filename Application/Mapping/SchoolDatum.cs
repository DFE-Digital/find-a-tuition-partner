namespace Application.Mapping;

public class SchoolDatum
{
    public int Urn { get; set; }
    public string Name { get; set; } = string.Empty;
    public string street { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Address3 { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public int EstablishmentTypeGroup { get; set; }
    public int EstablishmentStatus { get; set; }
    public int PhaseOfEducation { get; set; }
    public int LocalAuthorityCode { get; set; }
    public string LocalAuthorityDistrictCode { get; set; } = string.Empty;
}
