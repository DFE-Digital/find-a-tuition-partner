using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ILocalAuthorityDistrictRepository : IGenericRepository<LocalAuthorityDistrict>
{
    Task<IEnumerable<LocalAuthorityDistrict>> GetLocalAuthorityDistrictsAsync(CancellationToken cancellationToken = default);
    Task<IDictionary<string, LocalAuthorityDistrict>> GetLocalAuthorityDistrictDictionaryAsync(CancellationToken cancellationToken = default);
    Task<LocalAuthorityDistrict?> GetLocalAuthorityDistrictAsync(string? code, CancellationToken cancellationToken = default);
}