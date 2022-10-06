using Application;
using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Constants;

namespace Infrastructure.Factories
{
    internal class GeneralInformatioAboutSchoolsFactory : IGeneralInformationAboutSchoolsFactory
    {
        public School GetGeneralInformationAboutSchool(SchoolDatum schoolDatum, IDictionary<string, int> localAuthorityDistrictsIds)
        {  
            if (!localAuthorityDistrictsIds.TryGetValue(schoolDatum.LocalAuthorityDistrictCode.Trim(), out int localAuthorityDistrictCode))
            {
                localAuthorityDistrictCode = 0;
            }

            int setPhaseOfEducationToHandleDefaultOfZero = schoolDatum.PhaseOfEducation;
            if (schoolDatum.PhaseOfEducation == 0)
            {
                setPhaseOfEducationToHandleDefaultOfZero = (int)PhasesOfEducation.NotApplicable;
            }

            School generalInformationAboutSchools = new School
            {
                EstablishmentName = schoolDatum.Name,
                Urn = schoolDatum.Urn,
                Address = schoolDatum.Address,
                Postcode = schoolDatum.Postcode,
                PhaseOfEducationId = setPhaseOfEducationToHandleDefaultOfZero,
                EstablishmentTypeGroupId = schoolDatum.EstablishmentTypeGroup,
                EstablishmentStatusId = schoolDatum.EstablishmentStatus,
                LocalAuthorityId = schoolDatum.LocalAuthorityCode,
                LocalAuthorityDistrictId = localAuthorityDistrictCode
            };
            return generalInformationAboutSchools;
        }
    }
}
