using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LocalAuthorityDistrictRepository : GenericRepository<LocalAuthorityDistrict>, ILocalAuthorityDistrictRepository
{
    public LocalAuthorityDistrictRepository(NtpDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<LocalAuthorityDistrict>> GetLocalAuthorityDistrictsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LocalAuthorityDistricts.Include(e => e.LocalAuthority).OrderBy(e => e.Name).ToListAsync(cancellationToken);
    }

    public async Task<IDictionary<string, LocalAuthorityDistrict>> GetLocalAuthorityDistrictDictionaryAsync(CancellationToken cancellationToken = default)
    {
        return (await GetLocalAuthorityDistrictsAsync(cancellationToken)).ToDictionary(e => e.Code);
    }

    public async Task<LocalAuthorityDistrict?> GetLocalAuthorityDistrictAsync(string? code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        return await _context.LocalAuthorityDistricts.Include(e => e.LocalAuthority).SingleOrDefaultAsync(e => e.Code == code, cancellationToken);
    }
}