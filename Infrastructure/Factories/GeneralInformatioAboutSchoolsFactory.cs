using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Factories;
using Application.Mapping;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Factories
{
    internal class GeneralInformatioAboutSchoolsFactory : IGeneralInformationAboutSchoolsFactory
    {
        public GeneralInformationAboutSchools GetGeneralInformationAboutSchools(SchoolDatum schoolDatum, INtpDbContext dbContext, CancellationToken cancellationToken)
        {
            GeneralInformationAboutSchools generalInformationAboutSchools = new GeneralInformationAboutSchools
            {
                EstablishmentName = schoolDatum.Name,
                Urn = Int32.Parse(schoolDatum.Urn),
                Address = schoolDatum.Address,
                PhaseOfEducation = new PhaseOfEducation { Id = schoolDatum.PhaseOfEducation },
                EstablishmentTypeGroup = new EstablishmentTypeGroup { Id = schoolDatum.EstablishmentTypeGroup },
                EstablishmentStatus = new EstablishmentStatus { Id = schoolDatum.EstablishmentStatus },
                LocalEducationAuthority = new LocalAuthority { Id = schoolDatum.LocalEducationAuthorityCode },
                LocalAuthorityDistrict = new LocalAuthorityDistrict { Id = schoolDatum.LocalAuthorityDistrictCode }
            };

            return generalInformationAboutSchools;
        }
    }
}
