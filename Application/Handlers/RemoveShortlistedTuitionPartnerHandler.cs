namespace Application.Handlers;

public class RemoveShortlistedTuitionPartnerHandler : IRequestHandler<RemoveTuitionPartnerCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveShortlistedTuitionPartnerHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(RemoveTuitionPartnerCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.RemoveTuitionPartner(request.SeoUrl, request.LocalAuthorityName));
}