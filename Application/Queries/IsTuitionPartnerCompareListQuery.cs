namespace Application.Queries;

public record IsTuitionPartnerCompareListQuery(string TuitionPartnerSeoUrl) : IRequest<bool>;