using Domain.Search.ShortlistTuitionPartners;
using MediatR;

namespace Application.Commands;

public record AddTuitionPartnerToShortlistCommand(ShortlistedTuitionPartner ShortlistedTuitionPartner) : IRequest<int>;