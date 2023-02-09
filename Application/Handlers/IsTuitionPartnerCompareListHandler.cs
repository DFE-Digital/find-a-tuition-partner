using Application.Common.Interfaces;
using Application.Queries;

namespace Application.Handlers;

public class IsTuitionPartnerCompareListHandler : IRequestHandler<IsTuitionPartnerCompareListQuery, bool>
{
    private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;

    public IsTuitionPartnerCompareListHandler(ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService) =>
        _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;

    public Task<bool> Handle(IsTuitionPartnerCompareListQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerCompareListStorageService.IsTuitionPartnerCompareListed(request.TuitionPartnerSeoUrl));
}