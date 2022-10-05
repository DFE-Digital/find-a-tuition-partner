using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mapping;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Factories;

public interface IGeneralInformationAboutSchoolsFactory
{
    GeneralInformationAboutSchools GetGeneralInformationAboutSchools(SchoolDatum stream, INtpDbContext dbContext, CancellationToken cancellationToken);
}