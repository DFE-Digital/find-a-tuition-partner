using Application;
using Application.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LookupDataRepository : ILookupDataRepository
{
    private readonly INtpDbContext _dbContext;

    public LookupDataRepository(INtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }
}