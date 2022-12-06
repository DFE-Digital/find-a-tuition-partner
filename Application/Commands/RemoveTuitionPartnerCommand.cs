using MediatR;

namespace Application.Commands;

public record RemoveTuitionPartnerCommand(string SeoUrl, string LocalAuthority) : IRequest<int>;