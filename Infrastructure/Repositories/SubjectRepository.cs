using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(NtpDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Subjects.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }
}