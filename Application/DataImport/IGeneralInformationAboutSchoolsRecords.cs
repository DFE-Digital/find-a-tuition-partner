
using Application.Mapping;

namespace Application.DataImport;

public interface IGeneralInformationAboutSchoolsRecords
{
    Task<IReadOnlyCollection<SchoolDatum>> GetSchoolDataAsync(CancellationToken cancellationToken);
}