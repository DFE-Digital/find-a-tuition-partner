using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionTypeRepository : IGenericRepository<TuitionType>
{
    Task<IEnumerable<TuitionType>> GetTuitionTypesAsync(CancellationToken cancellationToken = default);
}