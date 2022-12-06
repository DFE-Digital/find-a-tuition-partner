using Domain.Search.ShortlistTuitionPartners;
using MediatR;

namespace Application.Commands;

public record AddAllTuitionPartnersToShortlistCommand(IEnumerable<ShortlistedTuitionPartner> TuitionPartners) : IRequest<int>;
