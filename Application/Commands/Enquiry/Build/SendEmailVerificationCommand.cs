using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmailStatus = Domain.Enums.EmailStatus;

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

    private readonly IUnitOfWork _unitOfWork;
    private readonly IProcessEmailsService _processEmailsService;
    private readonly ILogger<SendEmailVerificationCommandHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    private string? _environmentNameNonProduction;

    public SendEmailVerificationCommandHandler(
        IUnitOfWork unitOfWork,
        IProcessEmailsService processEmailsService,
        ILogger<SendEmailVerificationCommandHandler> logger,
        IHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _processEmailsService = processEmailsService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<int> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            validationResult = $"The {nameof(SendEmailVerificationCommand)} {validationResult}";
            _logger.LogError(validationResult);
            throw new ArgumentException(validationResult);
        }

        _environmentNameNonProduction = (_hostEnvironment.IsProduction() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null) ? string.Empty : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.ToString();

        var passcode = GeneratePasscode(request.PasscodeLength);

        var verificationEmailLog = GetVerificationEmailLog(request, passcode);

        _unitOfWork.EmailLogRepository.AddAsync(verificationEmailLog, cancellationToken);

        await _unitOfWork.Complete();

        try
        {
            await _processEmailsService.SendEmailAsync(verificationEmailLog.Id);
        }
        catch (Exception)
        {
            await CleanUpData(verificationEmailLog.Id);
            throw;
        }

        _logger.LogInformation("Email verification passcode sent: {passcode}", passcode);

        return passcode;
    }

    private static string? ValidateRequest(SendEmailVerificationCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return "Email is null";
        }

        return null;
    }

    private static int GeneratePasscode(int passcodeLength)
    {
        Random rnd = new();

        var minNumber = Convert.ToInt32(Math.Pow(10, passcodeLength - 1));
        var maxNumber = Convert.ToInt32(Math.Pow(10, passcodeLength)) - 1;

        return rnd.Next(minNumber, maxNumber);
    }

    private EmailLog GetVerificationEmailLog(SendEmailVerificationCommand request, int passcode)
    {
        var emailTemplateType = EmailTemplateType.EmailVerification;

        var verificationEmailPersonalisationLog = GetVerificationEmailPersonalisationLog(request, passcode);

        var createdDateTime = DateTime.UtcNow;

        var emailLog = new EmailLog()
        {
            CreatedDate = createdDateTime,
            ProcessFromDate = createdDateTime,
            FinishProcessingDate = createdDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = request.Email!,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = emailTemplateType.DisplayName().CreateNotifyEmailClientReference(_environmentNameNonProduction!),
            EmailStatusId = (int)EmailStatus.ToBeProcessed,
            EmailPersonalisationLogs = verificationEmailPersonalisationLog
        };

        return emailLog;
    }

    private static List<EmailPersonalisationLog> GetVerificationEmailPersonalisationLog(SendEmailVerificationCommand request, int passcode)
    {
        var personalisation = new Dictionary<string, dynamic>()
            {
                { EmailPasscodeKey, passcode },
                { SessionTimeoutMinutesKey, request.SessionTimeoutMinutes.ToString() }
            };

        personalisation.AddDefaultEmailPersonalisation(request.BaseServiceUrl!);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }

    private async Task CleanUpData(int emailLogId)
    {
        try
        {
            var emailLog = _unitOfWork.EmailLogRepository
                .GetById(emailLogId);

            _unitOfWork.EmailLogRepository.Remove(emailLog);

            await _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error thrown in SendEmailVerificationCommand CleanUpData");
        }
    }
}
