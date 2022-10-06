using Application;
using Application.Factories;
using Application.Mapping;
using Domain;

namespace Infrastructure.Factories
{
    internal class GeneralInformatioAboutSchoolsFactory : IGeneralInformationAboutSchoolsFactory
    {
        public School GetGeneralInformationAboutSchool(SchoolDatum schoolDatum, CancellationToken cancellationToken)
        {
            School generalInformationAboutSchools = new School
            {
                EstablishmentName = schoolDatum.Name,
                Urn = schoolDatum.Urn,
                Address = schoolDatum.Address,
                Postcode = schoolDatum.Postcode,
                PhaseOfEducationId = schoolDatum.PhaseOfEducation,
                EstablishmentTypeGroupId = schoolDatum.EstablishmentTypeGroup,
                EstablishmentStatusId = schoolDatum.EstablishmentStatus,
                LocalAuthorityId = schoolDatum.LocalAuthorityCode,
                //LocalAuthorityDistrictId = schoolDatum.LocalAuthorityDistrictCode
            };
            return generalInformationAboutSchools;
        }
    }
}
