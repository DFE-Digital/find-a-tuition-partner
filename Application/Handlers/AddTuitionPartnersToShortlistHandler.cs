namespace Application.Handlers;

public class AddTuitionPartnersToShortlistHandler : IRequestHandler<AddTuitionPartnersToShortlistCommand>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public AddTuitionPartnersToShortlistHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<Unit> Handle(AddTuitionPartnersToShortlistCommand request, CancellationToken cancellationToken)
    {
        _tuitionPartnerShortlistStorage.RemoveAllTuitionPartners();
        _tuitionPartnerShortlistStorage.AddTuitionPartners(request.ShortlistedTuitionPartnersSeoUrl);

        return Task.FromResult(Unit.Value);
    }
}