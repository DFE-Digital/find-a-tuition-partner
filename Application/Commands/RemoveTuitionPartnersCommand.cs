namespace Application.Commands;

public record RemoveTuitionPartnersCommand(IEnumerable<string> TuitionPartnersSeoUrl) : IRequest;