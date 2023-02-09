using Application.Common.Interfaces;

namespace Application.Handlers;

public class RemoveAllCompareListTuitionPartnersHandler : IRequestHandler<RemoveAllCompareListedTuitionPartnersCommand, bool>
{
    private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;

    public RemoveAllCompareListTuitionPartnersHandler(ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService) =>
        _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;


    public Task<bool> Handle(RemoveAllCompareListedTuitionPartnersCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerCompareListStorageService.RemoveAllTuitionPartners();
        return Task.FromResult(result);
    }
}