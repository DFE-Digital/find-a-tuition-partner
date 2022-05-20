using Domain;

namespace Application.Repositories;

public interface ILookupDataRepository
{
    Task<IEnumerable<Subject>> GetSubjectsAsync();
    Task<IEnumerable<TutorType>> GetTutorTypesAsync();
    Task<IEnumerable<TuitionType>> GetTuitionTypesAsync();
}