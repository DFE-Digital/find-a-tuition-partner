using Application.Common.Interfaces;
using Application.Repositories;
using Domain;

namespace Infrastructure.Services;

public class LookupDataService : ILookupDataService
{
    private readonly ILookupDataRepository _lookupDataRepository;

    public LookupDataService(ILookupDataRepository lookupDataRepository)
    {
        _lookupDataRepository = lookupDataRepository;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _lookupDataRepository.GetSubjectsAsync(cancellationToken);
    }
}