﻿using Application.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LookupDataRepository : ILookupDataRepository
{
    private readonly NtpDbContext _dbContext;

    public LookupDataRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subjects.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<TuitionType>> GetTuitionTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.TuitionTypes.OrderBy(e => e.Id).ToArrayAsync(cancellationToken);
    }
}