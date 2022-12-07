namespace Application.Commands;

public record AddTuitionPartnerToShortlistCommand(ShortlistedTuitionPartner ShortlistedTuitionPartner) : IRequest<int>;