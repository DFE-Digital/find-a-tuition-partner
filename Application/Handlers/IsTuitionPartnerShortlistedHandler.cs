using Application.Common.Interfaces;
using Application.Queries;

namespace Application.Handlers;

public class IsTuitionPartnerShortlistedHandler : IRequestHandler<IsTuitionPartnerShortlistedQuery, bool>
{
    private readonly ITuitionPartnerShortlistStorageService _tuitionPartnerShortlistStorageService;

    public IsTuitionPartnerShortlistedHandler(ITuitionPartnerShortlistStorageService tuitionPartnerShortlistStorageService) =>
        _tuitionPartnerShortlistStorageService = tuitionPartnerShortlistStorageService;

    public Task<bool> Handle(IsTuitionPartnerShortlistedQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorageService.IsTuitionPartnerShortlisted(request.TuitionPartnerSeoUrl));
}