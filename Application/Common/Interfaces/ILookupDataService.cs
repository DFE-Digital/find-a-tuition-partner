using Domain;

namespace Application.Common.Interfaces;

public interface ILookupDataService
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default);
}