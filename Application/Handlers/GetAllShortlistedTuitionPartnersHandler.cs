using Application.Common.Interfaces;
using Application.Queries;

namespace Application.Handlers;

public class
    GetAllShortlistedTuitionPartnersHandler : IRequestHandler<GetAllShortlistedTuitionPartnersQuery,
        IEnumerable<string>>
{
    private readonly ITuitionPartnerShortlistStorageService _tuitionPartnerShortlistStorageService;

    public GetAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorageService tuitionPartnerShortlistStorageService)
        => _tuitionPartnerShortlistStorageService = tuitionPartnerShortlistStorageService;

    public Task<IEnumerable<string>> Handle
        (GetAllShortlistedTuitionPartnersQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorageService.GetAllTuitionPartners());
}