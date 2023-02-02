using Domain;
using Domain.Search;

namespace Application;

public interface ILookupDataService
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default);
}