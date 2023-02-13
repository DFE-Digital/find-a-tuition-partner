using Application.Common.Interfaces;

namespace Application.Handlers;

public class AddTuitionPartnersToCompareListHandler : IRequestHandler<AddTuitionPartnersToCompareListCommand, bool>
{
    private readonly ITuitionPartnerCompareListStorageService _tuitionPartnerCompareListStorageService;

    public AddTuitionPartnersToCompareListHandler(ITuitionPartnerCompareListStorageService tuitionPartnerCompareListStorageService) =>
        _tuitionPartnerCompareListStorageService = tuitionPartnerCompareListStorageService;

    public Task<bool> Handle(AddTuitionPartnersToCompareListCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerCompareListStorageService.AddTuitionPartners(request.CompareListedTuitionPartnersSeoUrl);

        return Task.FromResult(result);
    }
}