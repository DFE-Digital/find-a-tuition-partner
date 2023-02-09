using Application.Common.Interfaces;
using Application.Queries;

namespace Application.Handlers;

public class
    GetAllCompareListTuitionPartnersHandler : IRequestHandler<GetAllCompareListTuitionPartnersQuery,
        IEnumerable<string>>
{
    private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;

    public GetAllCompareListTuitionPartnersHandler(ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService)
        => _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;

    public Task<IEnumerable<string>> Handle
        (GetAllCompareListTuitionPartnersQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerCompareListStorageService.GetAllTuitionPartners());
}