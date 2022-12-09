using Application.Queries;

namespace Application.Handlers;

public class
    GetAllShortlistedTuitionPartnersHandler : IRequestHandler<GetAllShortlistedTuitionPartnersQuery,
        IEnumerable<string>>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public GetAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage)
        => _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<IEnumerable<string>> Handle
        (GetAllShortlistedTuitionPartnersQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.GetAllTuitionPartners());
}