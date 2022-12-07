using Application.Queries;

namespace Application.Handlers;

public class GetShortlistedTuitionPartnersByLocalAuthorityHandler : IRequestHandler<
    GetShortlistedTuitionPartnersByLocalAuthorityQuery, IEnumerable<ShortlistedTuitionPartner>>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public GetShortlistedTuitionPartnersByLocalAuthorityHandler(
        ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage)
        => _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<IEnumerable<ShortlistedTuitionPartner>> Handle(
        GetShortlistedTuitionPartnersByLocalAuthorityQuery request, CancellationToken cancellationToken)
        => Task.FromResult(
            _tuitionPartnerShortlistStorage.GetTuitionPartnersByLocalAuthorityName(request.LocalAuthority));
}