using Application.Common.Interfaces;
using Application.Common.Models.Admin;

namespace Application.Commands.Admin;

public record ProcessEmailsCommand : IRequest<ProcessedEmailsModel>
{
    public string? NotificationId { get; set; } = null!;
}

public class ProcessEmailsCommandHandler : IRequestHandler<ProcessEmailsCommand, ProcessedEmailsModel>
{
    private readonly IProcessEmailsService _processEmailsService;

    public ProcessEmailsCommandHandler(IProcessEmailsService processEmailsService)
    {
        _processEmailsService = processEmailsService;
    }

    public async Task<ProcessedEmailsModel> Handle(ProcessEmailsCommand request, CancellationToken cancellationToken)
    {
        var result = new ProcessedEmailsModel();

        await _processEmailsService.ProcessAllEmailsAsync();

        return result;
    }
}
