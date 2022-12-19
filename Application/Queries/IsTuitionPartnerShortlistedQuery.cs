namespace Application.Queries;

public record IsTuitionPartnerShortlistedQuery(string TuitionPartnerSeoUrl) : IRequest<bool>;