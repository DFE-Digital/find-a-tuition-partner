using Application.Commands;
using Application.TuitionPartnerShortlistStorage.Interfaces;
using MediatR;

namespace Application.Handlers;

public class RemoveAllShortlistedTuitionPartnersHandler : IRequestHandler<RemoveAllTuitionPartnersCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveAllShortlistedTuitionPartnersHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;


    public Task<int> Handle(RemoveAllTuitionPartnersCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.RemoveAllTuitionPartners());
}