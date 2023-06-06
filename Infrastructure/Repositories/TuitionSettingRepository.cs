using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionSettingRepository : GenericRepository<TuitionSetting>, ITuitionSettingRepository
{
    public TuitionSettingRepository(NtpDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IEnumerable<TuitionSetting>> GetTuitionSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TuitionSettings.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }
}