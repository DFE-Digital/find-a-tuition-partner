﻿using Application.Mapping;
using Domain;

namespace Application.Factories;

public interface IGeneralInformationAboutSchoolsFactory
{
    School GetGeneralInformationAboutSchool(SchoolDatum stream, IDictionary<string, int> localAuthorityDistrictsIds, IDictionary<int, string> localAuthorityIds);
}