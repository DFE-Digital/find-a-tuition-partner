using Domain.Constants;

namespace Application.Mapping;

public class SchoolDatum
{
    public int Urn { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Address3 { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public int EstablishmentType { get; set; }
    public int EstablishmentTypeGroup { get; set; }
    public int EstablishmentStatus { get; set; }
    public int PhaseOfEducation { get; set; }
    public int LocalAuthorityCode { get; set; }
    public string LocalAuthorityDistrictCode { get; set; } = string.Empty;
    public int? EstablishmentNumber { get; set; } = null;
    public int? Ukprn { get; set; } = null;

    public bool IsValidForService()
    {
        if (EstablishmentStatus != (int)EstablishmentsStatus.Open && EstablishmentStatus != (int)EstablishmentsStatus.OpenButProposedToClose)
            return false;

        if (EstablishmentTypeGroup == (int)EstablishmentTypeGroups.WelshSchools)
            return false;

        if (EstablishmentType == EstablishmentTypes.Id.OffshoreSchools ||
            EstablishmentType == EstablishmentTypes.Id.BritishSchoolsOverseas ||
            EstablishmentType == EstablishmentTypes.Id.ServiceChildrensEducation)
            return false;

        if (!EstablishmentNumber.HasValue)
            return false;

        return true;
    }
}
