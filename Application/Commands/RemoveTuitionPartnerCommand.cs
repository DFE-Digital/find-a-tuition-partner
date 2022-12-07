namespace Application.Commands;

public record RemoveTuitionPartnerCommand(string SeoUrl, string LocalAuthorityName) : IRequest<int>;