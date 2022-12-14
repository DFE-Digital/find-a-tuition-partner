namespace Application.Handlers;

public class RemoveShortlistedTuitionPartnerHandler : IRequestHandler<RemoveTuitionPartnerCommand>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveShortlistedTuitionPartnerHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<Unit> Handle(RemoveTuitionPartnerCommand request, CancellationToken cancellationToken)
    {
        _tuitionPartnerShortlistStorage.RemoveTuitionPartner(request.SeoUrl);
        return Task.FromResult(Unit.Value);
    }
}