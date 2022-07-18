using System.ComponentModel;

namespace GiasPostcodeSearch;

public class SchoolDatum
{
    public string Urn { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public PhaseOfEducation PhaseOfEducation { get; set; }
    public string Postcode { get; init; } = string.Empty;
    public string LocalAuthorityCode { get; init; } = string.Empty;
    public string LocalAuthorityDistrictCode { get; init; } = string.Empty;

    public override string ToString()
    {
        return $"URN: {Urn}, Name: {Name}, Phase of Education: {PhaseOfEducation}, Postcode: {Postcode}, Local Authority code: {LocalAuthorityCode}, Local Authority District code: {LocalAuthorityDistrictCode}";
    }
}

public enum PhaseOfEducation
{
    [Description("Not applicable")]
    NotApplicable = 0,
    [Description("Nursery")]
    Nursery,
    [Description("Primary")]
    Primary,
    [Description("Secondary")]
    Secondary,
    [Description("16 plus")]
    SixteenPlus,
    [Description("All-through")]
    AllThrough,
    [Description("Middle deemed primary")]
    MiddleDeemedPrimary,
    [Description("Middle deemed secondary")]
    MiddleDeemedSecondary
}