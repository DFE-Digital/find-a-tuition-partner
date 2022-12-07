namespace Application.Queries;

public record GetShortlistedTuitionPartnersByLocalAuthorityQuery(string LocalAuthority) : IRequest<IEnumerable<ShortlistedTuitionPartner>>;