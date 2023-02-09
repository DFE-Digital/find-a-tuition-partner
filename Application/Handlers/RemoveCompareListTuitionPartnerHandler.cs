using Application.Common.Interfaces;

namespace Application.Handlers;

public class RemoveCompareListTuitionPartnerHandler : IRequestHandler<RemoveCompareListedTuitionPartnerCommand, bool>
{
    private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;

    public RemoveCompareListTuitionPartnerHandler(ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService) =>
        _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;

    public Task<bool> Handle(RemoveCompareListedTuitionPartnerCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerCompareListStorageService.RemoveTuitionPartner(request.SeoUrl);
        return Task.FromResult(result);
    }
}