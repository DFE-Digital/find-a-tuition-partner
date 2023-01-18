using Application.Queries;

namespace Application.Handlers;

public class IsTuitionPartnerShortlistedHandler : IRequestHandler<IsTuitionPartnerShortlistedQuery, bool>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public IsTuitionPartnerShortlistedHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<bool> Handle(IsTuitionPartnerShortlistedQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.IsTuitionPartnerShortlisted(request.TuitionPartnerSeoUrl));
}