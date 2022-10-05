namespace Application.Mapping;

public class SchoolDatum
{
    public string Urn { get; set;} = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int EstablishmentTypeGroup { get; set;}
    public int EstablishmentStatus { get; set;}
    public int PhaseOfEducation { get; set; }
    public int LocalEducationAuthorityCode { get; set; }
    public int  LocalAuthorityDistrictCode { get; set; }
}
