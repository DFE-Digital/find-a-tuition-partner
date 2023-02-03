using Application.Common.Interfaces;

namespace Application.Handlers;

public class AddTuitionPartnersToShortlistHandler : IRequestHandler<AddTuitionPartnersToShortlistCommand, bool>
{
    private readonly ITuitionPartnerShortlistStorageService _tuitionPartnerShortlistStorageService;

    public AddTuitionPartnersToShortlistHandler(ITuitionPartnerShortlistStorageService tuitionPartnerShortlistStorageService) =>
        _tuitionPartnerShortlistStorageService = tuitionPartnerShortlistStorageService;

    public Task<bool> Handle(AddTuitionPartnersToShortlistCommand request, CancellationToken cancellationToken)
    {
        var result = _tuitionPartnerShortlistStorageService.AddTuitionPartners(request.ShortlistedTuitionPartnersSeoUrl);

        return Task.FromResult(result);
    }
}