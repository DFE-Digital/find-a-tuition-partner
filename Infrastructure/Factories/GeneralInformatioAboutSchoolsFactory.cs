using Application;
using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Constants;
using NetTopologySuite.Geometries;

namespace Infrastructure.Factories
{
    internal class GeneralInformatioAboutSchoolsFactory : IGeneralInformationAboutSchoolsFactory
    {
        public School GetGeneralInformationAboutSchool(SchoolDatum schoolDatum, IDictionary<string, int> localAuthorityDistrictsIds, IDictionary<int, string> localAuthorityIds)
        {
            if (!localAuthorityDistrictsIds.TryGetValue(schoolDatum.LocalAuthorityDistrictCode.Trim(), out int localAuthorityDistrictCode))
            {
                localAuthorityDistrictCode = 0;
            }

            if (!localAuthorityIds.TryGetValue(schoolDatum.LocalAuthorityCode, out _))
            {
                schoolDatum.LocalAuthorityCode = 0;
            }

            if (schoolDatum.PhaseOfEducation == 0)
            {
                schoolDatum.PhaseOfEducation = (int)PhasesOfEducation.NotApplicable;
            }

            School generalInformationAboutSchools = new()
            {
                EstablishmentName = schoolDatum.Name,
                Urn = schoolDatum.Urn,
                Address = schoolDatum.Address,
                Postcode = schoolDatum.Postcode,
                PhaseOfEducationId = schoolDatum.PhaseOfEducation,
                EstablishmentTypeGroupId = schoolDatum.EstablishmentTypeGroup,
                EstablishmentStatusId = schoolDatum.EstablishmentStatus,
                LocalAuthorityId = schoolDatum.LocalAuthorityCode,
                LocalAuthorityDistrictId = localAuthorityDistrictCode
            };
            return generalInformationAboutSchools;
        }
    }
}
