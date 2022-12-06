using Application.Commands;
using Application.TuitionPartnerShortlistStorage.Interfaces;
using MediatR;

namespace Application.Handlers;

public class AddAllTuitionPartnersToShortlistHandler : IRequestHandler<AddAllTuitionPartnersToShortlistCommand, int>
{
    private readonly ITuitionPartnerShortlistStorage _tuitionPartnerShortlistStorage;

    public AddAllTuitionPartnersToShortlistHandler(ITuitionPartnerShortlistStorage tuitionPartnerShortlistStorage) =>
        _tuitionPartnerShortlistStorage = tuitionPartnerShortlistStorage;

    public Task<int> Handle(AddAllTuitionPartnersToShortlistCommand request, CancellationToken cancellationToken)
    {
        var counter = 0;
        foreach (var tuitionPartner in request.TuitionPartners)
        {
            _tuitionPartnerShortlistStorage.AddTuitionPartner(tuitionPartner);
            counter++;
        }

        return Task.FromResult(counter);
    }
}