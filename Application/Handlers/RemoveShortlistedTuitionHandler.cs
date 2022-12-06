using Application.Commands;
using Application.TuitionPartnerShortlistStorage.Interfaces;
using MediatR;

namespace Application.Handlers;

public class RemoveShortlistedTuitionHandler : IRequestHandler<RemoveTuitionPartnerCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public RemoveShortlistedTuitionHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(RemoveTuitionPartnerCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(_tuitionPartnerShortlistStorage.RemoveTuitionPartner(request.SeoUrl, request.LocalAuthority));
}