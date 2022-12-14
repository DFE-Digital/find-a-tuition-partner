namespace Application.Handlers;

public class RemoveAllShortlistedTuitionPartnersHandler : IRequestHandler<RemoveAllTuitionPartnersCommand>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;


    public Task<Unit> Handle(RemoveAllTuitionPartnersCommand request, CancellationToken cancellationToken)
    {
        _tuitionPartnerShortlistStorage.RemoveAllTuitionPartners();
        return Task.FromResult(Unit.Value);
    }
}