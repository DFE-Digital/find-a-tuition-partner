namespace Application.Handlers;

public class RemoveTuitionPartnersHandler : IRequestHandler<RemoveTuitionPartnersCommand>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveTuitionPartnersHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<Unit> Handle(RemoveTuitionPartnersCommand request, CancellationToken cancellationToken)
    {
        foreach (var seoUrl in request.TuitionPartnersSeoUrl)
        {
            if (!string.IsNullOrWhiteSpace(seoUrl))
                _tuitionPartnerShortlistStorage.RemoveTuitionPartner(seoUrl.Trim());
        }

        return Task.FromResult(Unit.Value);
    }
}