using Application.Commands;
using Application.TuitionPartnerShortlistStorage.Interfaces;
using MediatR;

namespace Application.Handlers;

public class AddTuitionPartnerToShortlistHandler : IRequestHandler<AddTuitionPartnerToShortlistCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public AddTuitionPartnerToShortlistHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(AddTuitionPartnerToShortlistCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.AddTuitionPartner(request.ShortlistedTuitionPartner));
}