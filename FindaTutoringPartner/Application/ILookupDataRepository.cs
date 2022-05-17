using Domain;

namespace Application;

public interface ILookupDataRepository
{
    Task<IEnumerable<Subject>> GetSubjectsAsync();
    Task<IEnumerable<TutorType>> GetTutorTypesAsync();
}