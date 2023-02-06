namespace Application.Commands;

public record RemoveShortlistedTuitionPartnerCommand(string SeoUrl) : IRequest<bool>;