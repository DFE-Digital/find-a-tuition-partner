namespace Application.Handlers;

public class AddTuitionPartnerToShortlistHandler : IRequestHandler<AddTuitionPartnerToShortlistCommand>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public AddTuitionPartnerToShortlistHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<Unit> Handle(AddTuitionPartnerToShortlistCommand request, CancellationToken cancellationToken)
    {
        _tuitionPartnerShortlistStorage.RemoveTuitionPartner(request.ShortlistedTuitionPartnerSeoUrl);

        _tuitionPartnerShortlistStorage.AddTuitionPartner(request.ShortlistedTuitionPartnerSeoUrl);
        return Task.FromResult(Unit.Value);
    }
}