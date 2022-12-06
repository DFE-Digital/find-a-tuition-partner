using MediatR;

namespace Application.Queries;

public record GetShortlistedTuitionPartnersLocalAuthorityQuery : IRequest<string?>;