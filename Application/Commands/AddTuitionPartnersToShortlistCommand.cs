namespace Application.Commands;

public record AddTuitionPartnersToShortlistCommand(IEnumerable<string> ShortlistedTuitionPartnersSeoUrl) : IRequest<bool>;
