using Domain;

namespace Application.Repositories;

public interface ILookupDataRepository
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TutorType>> GetTutorTypesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TuitionType>> GetTuitionTypesAsync(CancellationToken cancellationToken = default);
}