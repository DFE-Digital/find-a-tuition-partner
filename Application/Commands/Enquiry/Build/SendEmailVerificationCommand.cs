using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Enquiry.Build;

public record SendEmailVerificationCommand : IRequest<int>
{
    public string Email { get; set; } = null!;
    public int PasscodeLength { get; set; }
    public double SessionTimeoutMinutes { get; set; }
    public string BaseServiceUrl { get; set; } = null!;
}

public class SendEmailVerificationCommandHandler : IRequestHandler<SendEmailVerificationCommand, int>
{
    private const string EmailPasscodeKey = "email_passcode";
    private const string SessionTimeoutMinutesKey = "session_timeout_minutes";

    private readonly INotificationsClientService _notificationsClientService;
    private readonly ILogger<SendEmailVerificationCommandHandler> _logger;

    public SendEmailVerificationCommandHandler(
        INotificationsClientService notificationsClientService,
        ILogger<SendEmailVerificationCommandHandler> logger)
    {
        _notificationsClientService = notificationsClientService;
        _logger = logger;
    }

    public async Task<int> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var passcode = GeneratePasscode(request.PasscodeLength);

        var emailVerificationNotificationsRecipient = GetEmailVerificationNotificationsRecipient(request, passcode);

        await _notificationsClientService.SendEmailAsync(emailVerificationNotificationsRecipient, EmailTemplateType.EmailVerification);

        _logger.LogInformation("Email verification passcode: {passcode} for Email: {email}", passcode, request.Email);

        return passcode;
    }

    private static int GeneratePasscode(int passcodeLength)
    {
        Random rnd = new();

        var minNumber = Convert.ToInt32(Math.Pow(10, passcodeLength - 1));
        var maxNumber = Convert.ToInt32(Math.Pow(10, passcodeLength)) - 1;

        return rnd.Next(minNumber, maxNumber);
    }

    private static NotificationsRecipientDto GetEmailVerificationNotificationsRecipient(
        SendEmailVerificationCommand request, int passcode)
    {
        var personalisation = new Dictionary<string, dynamic>()
            {
                { EmailPasscodeKey, passcode },
                { SessionTimeoutMinutesKey, request.SessionTimeoutMinutes.ToString() }
            };

        var result = new NotificationsRecipientDto()
        {
            Email = request.Email,
            OriginalEmail = request.Email,
            EnquirerEmailForTestingPurposes = request.Email,
            Personalisation = personalisation
        };

        result.AddDefaultEmailDetails(request.BaseServiceUrl!, EmailTemplateType.EmailVerification);

        return result;
    }
}
