using Domain;

namespace Application.Repositories;

public interface IGeographyLookupRepository
{
    Task<IEnumerable<LocalAuthorityDistrict>> GetLocalAuthorityDistrictsAsync(CancellationToken cancellationToken = default);
    Task<IDictionary<string, LocalAuthorityDistrict>> GetLocalAuthorityDistrictDictionaryAsync(CancellationToken cancellationToken = default);
    Task<LocalAuthorityDistrict?> GetLocalAuthorityDistrictAsync(string code, CancellationToken cancellationToken = default);
}