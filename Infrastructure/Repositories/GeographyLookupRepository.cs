using Application.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GeographyLookupRepository : IGeographyLookupRepository
{
    private readonly NtpDbContext _dbContext;

    public GeographyLookupRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<LocalAuthorityDistrict>> GetLocalAuthorityDistrictsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalAuthorityDistricts.Include(e => e.LocalAuthority).OrderBy(e => e.Name).ToListAsync(cancellationToken);
    }

    public async Task<IDictionary<string, LocalAuthorityDistrict>> GetLocalAuthorityDistrictDictionaryAsync(CancellationToken cancellationToken = default)
    {
        return (await GetLocalAuthorityDistrictsAsync(cancellationToken)).ToDictionary(e => e.Code);
    }

    public async Task<LocalAuthorityDistrict?> GetLocalAuthorityDistrictAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalAuthorityDistricts.Include(e => e.LocalAuthority).SingleOrDefaultAsync(e => e.Code == code, cancellationToken);
    }
}