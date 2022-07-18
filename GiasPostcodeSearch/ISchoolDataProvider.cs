namespace GiasPostcodeSearch;

public interface ISchoolDataProvider
{
    Task<IReadOnlyCollection<SchoolDatum>> GetSchoolDataAsync(CancellationToken cancellationToken);
}