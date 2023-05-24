using Application.Mapping;
using Domain;

namespace Application.Factories;

public interface ISchoolsFactory
{
    School GetSchool(School school, SchoolDatum stream, IDictionary<string, int> localAuthorityDistrictsIds, IDictionary<int, string> localAuthorityIds);
}