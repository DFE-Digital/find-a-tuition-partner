using Application.Common.Interfaces;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Application.Commands.Enquiry.Build;

public record SendNotInterestedNotificationCommand(
    string SupportReferenceNumber,
    string TuitionPartnerSeoUrl,
    string TuitionPartnerEmailAddress,
    string TuitionPartnerName,
    string LocalAuthorityDistrict,
    string BaseServiceUrl) : IRequest<bool>
{ }

public class SendNotInterestedNotificationCommandHandler : IRequestHandler<SendNotInterestedNotificationCommand, bool>
{
    private const string EnquiryTpNameKey = "tp_name";
    private const string EnquiryLadNameKey = "local_area_district";
    private const string NotInterestedOutcomeFeedbackKey = "outcome_feedback";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IProcessEmailsService _processEmailsService;
    private readonly ILogger<SendNotInterestedNotificationCommandHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    private string? _environmentNameNonProduction;
    private int? _tpEnquiryResponseId;
    private int? _minsDelaySendingOutcomeEmailToTP;

    public SendNotInterestedNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        IProcessEmailsService processEmailsService,
        INotificationsClientService notificationsClientService,
        ILogger<SendNotInterestedNotificationCommandHandler> logger,
        IHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _processEmailsService = processEmailsService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<bool> Handle(SendNotInterestedNotificationCommand request, CancellationToken cancellationToken)
    {
        var tpEnquiryResponse = await _unitOfWork.EnquiryRepository.GetEnquiryResponse(request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
        _tpEnquiryResponseId = tpEnquiryResponse.Id;

        _environmentNameNonProduction = (_hostEnvironment.IsProduction() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null) ? string.Empty : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.ToString();

        _minsDelaySendingOutcomeEmailToTP = _processEmailsService.GetMinsDelaySendingOutcomeEmailToTP();

        var enquiryResponse = await UpdateEnquiryResponseWithNotInterestedEmail(request);

        if (enquiryResponse != null)
        {
            await _unitOfWork.Complete();

            _logger.LogInformation("Not interested email setup to be sent.  Email id: {emailId}, SupportReferenceNumber: {supportReferenceNumber}, TuitionPartnerSeoUrl: {tuitionPartnerSeoUrl} delayed by {minsDelaySendingOutcomeEmailToTP} mins",
                enquiryResponse.TuitionPartnerResponseNotInterestedEmailLogId, request.SupportReferenceNumber, request.TuitionPartnerSeoUrl, _minsDelaySendingOutcomeEmailToTP);
        }
        else
        {
            _logger.LogInformation("Call for subsequent not interested email setup to be sent.  For SupportReferenceNumber {SupportReferenceNumber} and TP {TuitionPartnerSeoUrl}",
                request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
        }

        return true;
    }

    private async Task<EnquiryResponse?> UpdateEnquiryResponseWithNotInterestedEmail(SendNotInterestedNotificationCommand request)
    {
        var enquiryResponse = await _unitOfWork.EnquiryResponseRepository
            .SingleOrDefaultAsync(x => x.Id == _tpEnquiryResponseId, "TuitionPartnerResponseNotInterestedEmailLog");

        if (enquiryResponse!.TuitionPartnerResponseNotInterestedEmailLog != null)
            return null;

        var emailTemplateType = EmailTemplateType.EnquiryOutcomeToTp;

        var personalisationLog = GetNotInterestedEmailPersonalisationLog(request);

        var createdDateTime = DateTime.UtcNow;
        var processDateTime = createdDateTime.AddMinutes(_minsDelaySendingOutcomeEmailToTP!.Value);

        enquiryResponse.TuitionPartnerResponseNotInterestedEmailLog = new EmailLog()
        {
            CreatedDate = createdDateTime,
            ProcessFromDate = processDateTime,
            FinishProcessingDate = processDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = request.TuitionPartnerEmailAddress,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = request.SupportReferenceNumber.CreateNotifyEnquiryClientReference(
                _environmentNameNonProduction!, emailTemplateType, request.TuitionPartnerName),
            EmailStatusId = (int)EmailStatus.DelayedEmail,
            EmailPersonalisationLogs = personalisationLog
        };

        return enquiryResponse;
    }

    private static List<EmailPersonalisationLog> GetNotInterestedEmailPersonalisationLog(SendNotInterestedNotificationCommand request)
    {
        var personalisation = new Dictionary<string, dynamic>()
            {
                { EnquiryTpNameKey, request.TuitionPartnerName },
                { EnquiryLadNameKey, request.LocalAuthorityDistrict },
                { NotInterestedOutcomeFeedbackKey, StringConstants.NotInterestedTextIfNoReasonSupplied }
            };

        personalisation.AddDefaultEnquiryPersonalisation(request.SupportReferenceNumber, request.BaseServiceUrl);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }
}
