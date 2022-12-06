using Application.Queries;
using Application.TuitionPartnerShortlistStorage.Interfaces;
using Domain.Search.ShortlistTuitionPartners;
using MediatR;

namespace Application.Handlers;

public class
    GetAllShortlistedTuitionPartnersHandler : IRequestHandler<GetAllShortlistedTuitionPartnersQuery,
        IEnumerable<ShortlistedTuitionPartner>>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public GetAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) => _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<IEnumerable<ShortlistedTuitionPartner>> Handle
        (GetAllShortlistedTuitionPartnersQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.GetAllTuitionPartners());
}