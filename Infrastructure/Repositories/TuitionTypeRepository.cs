using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionTypeRepository : GenericRepository<TuitionType>, ITuitionTypeRepository
{
    public TuitionTypeRepository(NtpDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IEnumerable<TuitionType>> GetTuitionTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TuitionTypes.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }
}