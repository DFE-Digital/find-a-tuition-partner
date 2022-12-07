namespace Application.Commands;

public record RemoveTuitionPartnersByLocalAuthorityCommand(string LocalAuthorityName) : IRequest<int>;