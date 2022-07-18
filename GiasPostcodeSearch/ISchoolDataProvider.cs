namespace GiasPostcodeSearch;

public interface ISchoolDataProvider
{
    Task<IEnumerable<SchoolDatum>> GetSchoolDataAsync(CancellationToken cancellationToken);
}