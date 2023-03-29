using Application.Common.Interfaces;
using Domain;

namespace Infrastructure.Services;

public class LookupDataService : ILookupDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public LookupDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SubjectRepository.GetSubjectsAsync(cancellationToken);
    }
}