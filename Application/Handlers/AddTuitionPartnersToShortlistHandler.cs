namespace Application.Handlers;

public class AddTuitionPartnersToShortlistHandler : IRequestHandler<AddTuitionPartnersToShortlistCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public AddTuitionPartnersToShortlistHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(AddTuitionPartnersToShortlistCommand request, CancellationToken cancellationToken)
    {
        var counter = 0;

        _tuitionPartnerShortlistStorage.RemoveAllTuitionPartnersByLocalAuthority(
            request.ShortlistedTuitionPartners.First().LocalAuthorityName.Trim());
        foreach (var shortlistedTuitionPartner in request.ShortlistedTuitionPartners)
        {
            _tuitionPartnerShortlistStorage.AddTuitionPartner(shortlistedTuitionPartner);
            counter++;
        }

        return Task.FromResult(counter);
    }
}