namespace Application.Commands;

public record AddTuitionPartnersToShortlistCommand(IEnumerable<ShortlistedTuitionPartner> ShortlistedTuitionPartners) : IRequest<int>;
