using Application.Common.Interfaces;

namespace Application.Handlers;

public class RemoveAllShortlistedTuitionPartnersHandler : IRequestHandler<RemoveAllShortlistedTuitionPartnersCommand, bool>
{
    private readonly ITuitionPartnerShortlistStorageService _tuitionPartnerShortlistStorageService;

    public RemoveAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorageService tuitionPartnerShortlistStorageService) =>
        _tuitionPartnerShortlistStorageService = tuitionPartnerShortlistStorageService;


    public Task<bool> Handle(RemoveAllShortlistedTuitionPartnersCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerShortlistStorageService.RemoveAllTuitionPartners();
        return Task.FromResult(result);
    }
}