using Application.Common.Interfaces;
using Application.Common.Models.Admin;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Admin;

public record ProcessEmailsCommand : IRequest<ProcessedEmailsModel>
{
    public string? NotificationId { get; set; } = null!;
}

public class ProcessEmailsCommandHandler : IRequestHandler<ProcessEmailsCommand, ProcessedEmailsModel>
{
    private readonly INotificationsClientService _notificationsClientService;
    private readonly IProcessEmailsService _processEmailsService;
    private readonly ILogger<ProcessEmailsCommandHandler> _logger;

    public ProcessEmailsCommandHandler(INotificationsClientService notificationsClientService, IProcessEmailsService processEmailsService,
        ILogger<ProcessEmailsCommandHandler> logger)
    {
        _notificationsClientService = notificationsClientService;
        _processEmailsService = processEmailsService;
        _logger = logger;
    }

    public async Task<ProcessedEmailsModel> Handle(ProcessEmailsCommand request, CancellationToken cancellationToken)
    {
        var result = new ProcessedEmailsModel();

        _logger.LogInformation("Processing emails...");

        //TESTING CODE FOR SPIKE...

        try
        {
            if (!string.IsNullOrWhiteSpace(request!.NotificationId))
            {
                var notificationOutcome = await _notificationsClientService.GetNotificationById(request!.NotificationId);
            }
        }
        catch { } //TODO - suppress for now while testing, prob let it throw normally?

        var notificationOutcomes = await _notificationsClientService.GetNotifications();

        await _processEmailsService.SendEmail("abc");


        return result;
    }
}
