namespace Application.Commands;

public record RemoveCompareListedTuitionPartnerCommand(string SeoUrl) : IRequest<bool>;