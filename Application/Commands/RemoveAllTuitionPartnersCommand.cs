using MediatR;

namespace Application.Commands;

public record RemoveAllTuitionPartnersCommand() : IRequest<int>;