namespace Application.Commands;

public record AddTuitionPartnerToShortlistCommand(string ShortlistedTuitionPartnerSeoUrl) : IRequest;