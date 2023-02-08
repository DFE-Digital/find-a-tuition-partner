using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default);
}