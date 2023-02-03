using Application.Common.Interfaces;

namespace Application.Handlers;

public class RemoveShortlistedTuitionPartnerHandler : IRequestHandler<RemoveShortlistedTuitionPartnerCommand, bool>
{
    private readonly ITuitionPartnerShortlistStorageService _tuitionPartnerShortlistStorageService;

    public RemoveShortlistedTuitionPartnerHandler(ITuitionPartnerShortlistStorageService tuitionPartnerShortlistStorageService) =>
        _tuitionPartnerShortlistStorageService = tuitionPartnerShortlistStorageService;

    public Task<bool> Handle(RemoveShortlistedTuitionPartnerCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerShortlistStorageService.RemoveTuitionPartner(request.SeoUrl);
        return Task.FromResult(result);
    }
}