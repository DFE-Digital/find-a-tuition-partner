using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Constants;

namespace Infrastructure.Factories
{
    internal class SchoolsFactory : ISchoolsFactory
    {
        public School GetSchool(SchoolDatum schoolDatum, IDictionary<string, int> localAuthorityDistrictsIds, IDictionary<int, string> localAuthorityIds)
        {
            if (!localAuthorityDistrictsIds.TryGetValue(schoolDatum.LocalAuthorityDistrictCode.Trim(), out int localAuthorityDistrictCode))
            {
                localAuthorityDistrictCode = 0;
            }

            if (!localAuthorityIds.TryGetValue(schoolDatum.LocalAuthorityCode, out _))
            {
                schoolDatum.LocalAuthorityCode = 0;
            }

            if (schoolDatum.PhaseOfEducation == 0) // Convert from 0 to 9999 as as index 0 cannot be saved in Entity Framework
            {
                schoolDatum.PhaseOfEducation = (int)PhasesOfEducation.NotApplicable;
            }

            School generalInformationAboutSchools = new()
            {
                EstablishmentName = schoolDatum.Name,
                Urn = schoolDatum.Urn,
                Address = CreateAddress(schoolDatum),
                Postcode = schoolDatum.Postcode,
                PhaseOfEducationId = schoolDatum.PhaseOfEducation,
                EstablishmentTypeGroupId = schoolDatum.EstablishmentTypeGroup,
                EstablishmentStatusId = schoolDatum.EstablishmentStatus,
                LocalAuthorityId = schoolDatum.LocalAuthorityCode,
                LocalAuthorityDistrictId = localAuthorityDistrictCode
            };
            return generalInformationAboutSchools;
        }

        private static string CreateAddress(SchoolDatum schoolDatum)
        {
            List<string> addresses = new();

            if (!string.IsNullOrEmpty(schoolDatum.street))
            {
                addresses.Add(schoolDatum.street);
            }
            if (!string.IsNullOrEmpty(schoolDatum.Locality))
            {
                addresses.Add(schoolDatum.Locality);
            }

            if (!string.IsNullOrEmpty(schoolDatum.Address3))
            {
                addresses.Add(schoolDatum.Address3);
            }

            if (!string.IsNullOrEmpty(schoolDatum.Town))
            {
                addresses.Add(schoolDatum.Town);
            }

            if (!string.IsNullOrEmpty(schoolDatum.County))
            {
                addresses.Add(schoolDatum.County);
            }

            return String.Join(",", addresses.ToArray());
        }
    }
}
