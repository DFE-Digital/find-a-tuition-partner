namespace Application.Handlers;

public class
    RemoveAllTuitionPartnersByLocalAuthorityHandler : IRequestHandler<RemoveTuitionPartnersByLocalAuthorityCommand,
        int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveAllTuitionPartnersByLocalAuthorityHandler(
        ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(RemoveTuitionPartnersByLocalAuthorityCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(_tuitionPartnerShortlistStorage.RemoveAllTuitionPartnersByLocalAuthority(request.LocalAuthorityName));
}