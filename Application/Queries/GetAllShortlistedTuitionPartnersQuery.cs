using Domain.Search.ShortlistTuitionPartners;
using MediatR;

namespace Application.Queries;

public record GetAllShortlistedTuitionPartnersQuery() : IRequest<IEnumerable<ShortlistedTuitionPartner>>
{

}