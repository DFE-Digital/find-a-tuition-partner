using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Constants;

namespace Infrastructure.Factories
{
    internal class SchoolsFactory : ISchoolsFactory
    {
        public School GetSchool(School school, SchoolDatum schoolDatum, IDictionary<string, int> localAuthorityDistrictsIds, IDictionary<int, string> localAuthorityIds)
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

            school.EstablishmentName = schoolDatum.Name;
            school.Urn = schoolDatum.Urn;
            school.Address = CreateAddress(schoolDatum);
            school.Postcode = schoolDatum.Postcode;
            school.PhaseOfEducationId = schoolDatum.PhaseOfEducation;
            school.EstablishmentTypeGroupId = schoolDatum.EstablishmentTypeGroup;
            school.EstablishmentStatusId = schoolDatum.EstablishmentStatus;
            school.LocalAuthorityId = schoolDatum.LocalAuthorityCode;
            school.LocalAuthorityDistrictId = localAuthorityDistrictCode;
            school.EstablishmentNumber = schoolDatum.EstablishmentNumber!.Value;
            school.Ukprn = schoolDatum.Ukprn;
            school.IsActive = true;

            return school;
        }

        private static string CreateAddress(SchoolDatum schoolDatum)
        {
            List<string> addresses = new();

            if (!string.IsNullOrEmpty(schoolDatum.Street))
            {
                addresses.Add(schoolDatum.Street);
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

            return string.Join(", ", addresses.ToArray());
        }
    }
}
