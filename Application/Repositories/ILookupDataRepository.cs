using Domain;

namespace Application.Repositories;

public interface ILookupDataRepository
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default);
}