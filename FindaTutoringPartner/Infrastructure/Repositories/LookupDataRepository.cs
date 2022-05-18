using Application;
using Application.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LookupDataRepository : ILookupDataRepository
{
    private NtpDbContext _dbContext;

    public LookupDataRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync()
    {
        return await _dbContext.Subjects.OrderBy(e => e.Id).ToArrayAsync();
    }

    public async Task<IEnumerable<TutorType>> GetTutorTypesAsync()
    {
        return await _dbContext.TutorTypes.OrderBy(e => e.Id).ToArrayAsync();
    }
}