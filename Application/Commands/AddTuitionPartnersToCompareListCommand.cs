namespace Application.Commands;

public record AddTuitionPartnersToCompareListCommand(IEnumerable<string> CompareListedTuitionPartnersSeoUrl) : IRequest<bool>;
