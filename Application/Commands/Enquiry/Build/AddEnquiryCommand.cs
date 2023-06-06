using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Domain.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmailStatus = Domain.Enums.EmailStatus;
using TuitionType = Domain.Enums.TuitionType;

namespace Application.Commands.Enquiry.Build;

public record AddEnquiryCommand : IRequest<SubmittedConfirmationModel>
{
    public EnquiryBuildModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, SubmittedConfirmationModel>
{
    private const string EnquiryNumberOfTpsContactedKey = "number_of_tps_contacted";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";
    private const string EnquiryTpNameKey = "tp_name";
    private const string EnquiryLadNameKey = "local_area_district";
    private const string EnquiryResponseFormLinkKey = "link_to_tp_response_form";
    private const string EnquiryDaysToRespond = "number_days_to_respond";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRandomTokenGenerator _randomTokenGenerator;
    private readonly IProcessEmailsService _processEmailsService;
    private readonly IGenerateReferenceNumber _generateReferenceNumber;
    private readonly ILogger<AddEnquiryCommandHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    private readonly DateTime _createdDateTime = DateTime.UtcNow;
    private string? _environmentNameNonProduction;
    private string? _enquiryReferenceNumber;
    private string? _enquirerToken;
    private List<TuitionPartnerMagicLinkModel>? _tpMagicLinkModelList;
    private bool _sendTuitionPartnerEmailsWhenEnquirerDelivered = true;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork,
        IRandomTokenGenerator randomTokenGenerator,
        IProcessEmailsService processEmailsService,
        IGenerateReferenceNumber generateReferenceNumber,
        ILogger<AddEnquiryCommandHandler> logger,
        IHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _randomTokenGenerator = randomTokenGenerator;
        _processEmailsService = processEmailsService;
        _generateReferenceNumber = generateReferenceNumber;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<SubmittedConfirmationModel> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        var result = new SubmittedConfirmationModel();

        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            validationResult = $"The {nameof(AddEnquiryCommand)} {validationResult}";
            _logger.LogError(validationResult);
            throw new ArgumentException(validationResult);
        }

        _sendTuitionPartnerEmailsWhenEnquirerDelivered = _processEmailsService.SendTuitionPartnerEmailsWhenEnquirerDelivered();
        _environmentNameNonProduction = (_hostEnvironment.IsProduction() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null) ? string.Empty : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.ToString();
        _enquiryReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber();
        _enquirerToken = _randomTokenGenerator.GenerateRandomToken();

        var enquiry = GetEnquiry(request);

        try
        {
            _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

            await _unitOfWork.Complete();
        }
        catch (DbUpdateException ex)
        {
            enquiry = await HandleDbUpdateException(ex, request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to save the enquiry.");
            throw;
        }

        _logger.LogInformation("Enquiry successfully created with magic links. EnquiryId: {enquiryId}", enquiry.Id);

        await ProcessEmailsAsync(enquiry);

        //Result population
        result.SupportReferenceNumber = _enquiryReferenceNumber;
        result.EnquirerMagicLink = _enquirerToken;

        result.TuitionPartnerMagicLinks = _tpMagicLinkModelList!;
        result.TuitionPartnerMagicLinksCount = _tpMagicLinkModelList!.Count;

        return result;
    }

    private static string? ValidateRequest(AddEnquiryCommand request)
    {
        if (request.Data == null)
        {
            return "Data is null";
        }

        if (request.Data.TuitionPartnersForEnquiry == null || request.Data.TuitionPartnersForEnquiry.Count == 0)
        {
            return "Data.TuitionPartnersForEnquiry count is 0";
        }

        if (string.IsNullOrWhiteSpace(request.Data.Postcode))
        {
            return "Data.Postcode is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TuitionPartnersForEnquiry.LocalAuthorityDistrictName))
        {
            return "Data.LocalAuthorityDistrictName is null or empty";
        }

        if (request.Data.Subjects == null || !request.Data.Subjects!.Any() || !request.Data.Subjects!.ParseKeyStageSubjects().Any())
        {
            return "Data.Subjects count is 0";
        }

        if (!request.Data.SchoolId.HasValue)
        {
            return "Data.SchoolId is null";
        }

        if (string.IsNullOrWhiteSpace(request.Data.Email))
        {
            return "Data.Email is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogistics))
        {
            return "Data.TutoringLogistics is null or empty";
        }

        return null;
    }

    private Domain.Enquiry GetEnquiry(AddEnquiryCommand request)
    {
        var enquirerEnquirySubmittedEmailLog = GetEnquirerEnquirySubmittedEmailLog(request);
        var enquirerMagicLink = new MagicLink()
        {
            Token = _enquirerToken!
        };

        var tuitionPartnerEnquiries = GetTuitionPartnerEnquiries(request);

        //Populate this to return before merge below
        _tpMagicLinkModelList = new List<TuitionPartnerMagicLinkModel>();
        tuitionPartnerEnquiries.ForEach(x =>
            _tpMagicLinkModelList.Add(new TuitionPartnerMagicLinkModel()
            {
                TuitionPartnerSeoUrl = request.Data!.TuitionPartnersForEnquiry!.Results.FirstOrDefault(y => y.Id == x.TuitionPartnerId)!.SeoUrl,
                Email = x.TuitionPartnerEnquirySubmittedEmailLog.EmailAddress,
                MagicLinkToken = x.MagicLink.Token!
            }));

        //See if emails are merged, for non-production testing
        var tpEmailLogs = tuitionPartnerEnquiries.Select(x => x.TuitionPartnerEnquirySubmittedEmailLog).ToList();
        var mergedEmailLog = _processEmailsService.MergeEmailForTesting(
            tpEmailLogs,
            new List<string>() { EnquiryTpNameKey, EnquiryResponseFormLinkKey });
        if (mergedEmailLog != null)
        {
            mergedEmailLog.ClientReferenceNumber = _enquiryReferenceNumber!.CreateNotifyClientReference(_environmentNameNonProduction!, EmailTemplateType.EnquirySubmittedToTp);
            tuitionPartnerEnquiries.ForEach(x => x.TuitionPartnerEnquirySubmittedEmailLog = mergedEmailLog);
        }

        //Add email dependency
        SetTuitionPartnerEmailsActivatedOnEnquirerDelivery(
            enquirerEnquirySubmittedEmailLog,
            mergedEmailLog == null ? tpEmailLogs : new List<EmailLog>() { mergedEmailLog });

        //Populate and save the enquiry
        return new Domain.Enquiry()
        {
            SchoolId = request.Data?.SchoolId!,
            Email = request.Data?.Email!,
            TutoringLogistics = request.Data?.TutoringLogistics!,
            SENDRequirements = request.Data?.SENDRequirements ?? null,
            AdditionalInformation = request.Data?.AdditionalInformation ?? null,
            TuitionPartnerEnquiry = tuitionPartnerEnquiries,
            SupportReferenceNumber = _enquiryReferenceNumber!,
            KeyStageSubjectEnquiry = GetKeyStageSubjectsEnquiry(request.Data!.Subjects!.ParseKeyStageSubjects()),
            PostCode = request.Data!.Postcode!,
            LocalAuthorityDistrict = request.Data!.TuitionPartnersForEnquiry!.LocalAuthorityDistrictName!,
            TuitionTypeId = GetTuitionTypeId(request.Data!.TuitionType),
            MagicLink = enquirerMagicLink,
            EnquirerEnquirySubmittedEmailLog = enquirerEnquirySubmittedEmailLog,
            CreatedAt = _createdDateTime
        };
    }

    private EmailLog GetEnquirerEnquirySubmittedEmailLog(AddEnquiryCommand request)
    {
        var emailTemplateType = EmailTemplateType.EnquirySubmittedConfirmationToEnquirer;

        var enquirerEnquirySubmittedEmailPersonalisationLog = GetEnquirerEnquirySubmittedEmailPersonalisationLog(request);

        var emailLog = new EmailLog()
        {
            CreatedDate = _createdDateTime,
            ProcessFromDate = _createdDateTime,
            FinishProcessingDate = _createdDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = request.Data?.Email!,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(request.Data?.Email!),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = _enquiryReferenceNumber!.CreateNotifyClientReference(_environmentNameNonProduction!, emailTemplateType),
            EmailStatusId = (int)EmailStatus.ToBeProcessed,
            EmailPersonalisationLogs = enquirerEnquirySubmittedEmailPersonalisationLog
        };

        return emailLog;
    }

    private List<EmailPersonalisationLog> GetEnquirerEnquirySubmittedEmailPersonalisationLog(AddEnquiryCommand request)
    {
        var enquirerViewAllResponsesPageLink = $"{request.Data?.BaseServiceUrl}/enquiry/{_enquiryReferenceNumber}?Token={_enquirerToken}";

        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryNumberOfTpsContactedKey, request.Data!.TuitionPartnersForEnquiry!.Results!.Count().ToString() },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLink },
            { EnquiryDaysToRespond, IntegerConstants.EnquiryDaysToRespond }
        };

        personalisation.AddDefaultEnquiryPersonalisation(_enquiryReferenceNumber!, request.Data!.BaseServiceUrl!, _createdDateTime);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }

    private List<TuitionPartnerEnquiry> GetTuitionPartnerEnquiries(AddEnquiryCommand request)
    {
        var tuitionPartnerResults = request.Data!.TuitionPartnersForEnquiry!.Results;

        var tuitionPartnerEnquiries = tuitionPartnerResults.Select(tuitionPartnerResult =>
        {
            var token = _randomTokenGenerator.GenerateRandomToken();
            return new TuitionPartnerEnquiry()
            {
                TuitionPartnerId = tuitionPartnerResult.Id,
                MagicLink = new MagicLink()
                {
                    Token = token
                },
                ResponseCloseDate = _createdDateTime.AddDays(IntegerConstants.EnquiryDaysToRespond),
                TuitionPartnerEnquirySubmittedEmailLog = GetTuitionPartnerEnquirySubmittedEmailLog(request, tuitionPartnerResult, token)
            };
        }).ToList();

        return tuitionPartnerEnquiries;
    }

    private EmailLog GetTuitionPartnerEnquirySubmittedEmailLog(AddEnquiryCommand request, TuitionPartnerResult tuitionPartnerResult, string tuitionPartnerToken)
    {
        var emailTemplateType = EmailTemplateType.EnquirySubmittedToTp;

        var tuitionPartnerEnquirySubmittedEmailPersonalisationLog = GetTuitionPartnerEnquirySubmittedEmailPersonalisationLog(request,
            tuitionPartnerResult, tuitionPartnerToken);

        var emailLog = new EmailLog()
        {
            CreatedDate = _createdDateTime,
            ProcessFromDate = _sendTuitionPartnerEmailsWhenEnquirerDelivered ? null : _createdDateTime,
            FinishProcessingDate = _createdDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = tuitionPartnerResult.Email,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(request.Data?.Email!),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = _enquiryReferenceNumber!.CreateNotifyClientReference(_environmentNameNonProduction!, emailTemplateType, tuitionPartnerResult.Name),
            EmailStatusId = _sendTuitionPartnerEmailsWhenEnquirerDelivered ? (int)EmailStatus.WaitingToBeTriggered : (int)EmailStatus.ToBeProcessed,
            EmailPersonalisationLogs = tuitionPartnerEnquirySubmittedEmailPersonalisationLog
        };

        return emailLog;
    }

    private List<EmailPersonalisationLog> GetTuitionPartnerEnquirySubmittedEmailPersonalisationLog(
        AddEnquiryCommand request,
        TuitionPartnerResult tuitionPartnerResult,
        string token)
    {
        var tuitionPartnerResponsesPageLink = $"{request.Data!.BaseServiceUrl}/enquiry-response/{tuitionPartnerResult.Name.ToSeoUrl()}/{_enquiryReferenceNumber}?Token={token}";

        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTpNameKey, tuitionPartnerResult.Name },
            { EnquiryResponseFormLinkKey, tuitionPartnerResponsesPageLink },
            { EnquiryLadNameKey, request!.Data!.TuitionPartnersForEnquiry!.LocalAuthorityDistrictName! },
            { EnquiryDaysToRespond, IntegerConstants.EnquiryDaysToRespond }
        };

        personalisation.AddDefaultEnquiryPersonalisation(_enquiryReferenceNumber!, request.Data!.BaseServiceUrl!, _createdDateTime);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }

    private void SetTuitionPartnerEmailsActivatedOnEnquirerDelivery(EmailLog enquirerEmailLog, List<EmailLog> tpEmailLogs)
    {
        if (_sendTuitionPartnerEmailsWhenEnquirerDelivered &&
            enquirerEmailLog != null &&
            tpEmailLogs != null &&
            tpEmailLogs.Any())
        {
            enquirerEmailLog.EmailsActivatedByThisEmail = new List<EmailTriggerActivation>();
            foreach (var tpEmailLog in tpEmailLogs)
            {
                enquirerEmailLog.EmailsActivatedByThisEmail.Add(new EmailTriggerActivation()
                {
                    ActivateEmailLog = tpEmailLog,
                });
            }
        }
    }

    private static int? GetTuitionTypeId(TuitionType? tuitionType)
    {
        return tuitionType switch
        {
            null => null,
            TuitionType.InSchool => (int)TuitionType.InSchool,
            TuitionType.Online => (int)TuitionType.Online,
            _ => null
        };
    }

    private static List<KeyStageSubjectEnquiry> GetKeyStageSubjectsEnquiry(
        IEnumerable<KeyStageSubject> keyStageSubjects)
    {
        var keyStageSubjectEnquiry = new List<KeyStageSubjectEnquiry>();

        foreach (var (keyStageId, subjectId) in keyStageSubjects.ToArray().GetIdsForKeyStageSubjects())
        {
            keyStageSubjectEnquiry.Add(new KeyStageSubjectEnquiry()
            {
                KeyStageId = keyStageId,
                SubjectId = subjectId
            });
        }

        return keyStageSubjectEnquiry;
    }

    private async Task<Domain.Enquiry> HandleDbUpdateException(DbUpdateException ex,
        AddEnquiryCommand request,
        CancellationToken cancellationToken)
    {
        var dataSaved = false;
        var retryAttempt = 0;
        var updatedEnquiry = new Domain.Enquiry();

        while (!dataSaved)
        {
            retryAttempt++;

            if (ex.InnerException != null &&
                (ex.InnerException.Message.Contains("duplicate key") ||
                 ex.InnerException.Message.Contains("unique constraint") ||
                 ex.InnerException.Message.Contains("violates unique constraint")))
            {
                _logger.LogError(
                    ex,
                    "Violation on unique constraint. Support Reference Number: {referenceNumber}.  Next retry attempt number: {retryAttempt}",
                    _enquiryReferenceNumber, retryAttempt);

                _unitOfWork.EnquiryRepository.RollbackChanges();

                _enquiryReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber();
                _logger.LogInformation("Generating new support reference number: {referenceNumber}",
                    _enquiryReferenceNumber);

                updatedEnquiry = GetEnquiry(request);
                _unitOfWork.EnquiryRepository.AddAsync(updatedEnquiry, cancellationToken);

                try
                {
                    dataSaved = await _unitOfWork.Complete();
                }
                catch (DbUpdateException retryEx)
                {
                    if (retryAttempt > 10)
                    {
                        throw;
                    }
                    ex = retryEx;
                }
            }
            else
            {
                throw ex;
            }
        }
        return updatedEnquiry;
    }

    private async Task ProcessEmailsAsync(Domain.Enquiry enquiry)
    {
        try
        {
            await _processEmailsService.SendEmailAsync(enquiry.EnquirerEnquirySubmittedEmailLog.Id);
        }
        catch (Exception)
        {
            await CleanUpData(enquiry);
            throw;
        }

        try
        {
            if (!_sendTuitionPartnerEmailsWhenEnquirerDelivered)
            {
                var emailLogIds = enquiry.TuitionPartnerEnquiry.Select(x => x.TuitionPartnerEnquirySubmittedEmailLogId).ToArray();
                await _processEmailsService.SendEmailsAsync(emailLogIds!);
            }
        }
        catch
        {
            //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in the services
        }
    }

    private async Task CleanUpData(Domain.Enquiry enquiry)
    {
        try
        {
            var tpRecords = await _unitOfWork.TuitionPartnerEnquiryRepository
                .GetAllAsync(x => x.Enquiry.SupportReferenceNumber == enquiry.SupportReferenceNumber, "Enquiry,MagicLink,TuitionPartnerEnquirySubmittedEmailLog",
                    true);
            tpRecords.Select(x => x.MagicLink).ToList().ForEach(x => _unitOfWork.MagicLinkRepository.Remove(x));
            _unitOfWork.MagicLinkRepository.Remove(enquiry.MagicLink);

            tpRecords.Select(x => x.TuitionPartnerEnquirySubmittedEmailLog).ToList().ForEach(x => _unitOfWork.EmailLogRepository.Remove(x));
            _unitOfWork.EmailLogRepository.Remove(enquiry.EnquirerEnquirySubmittedEmailLog);

            _unitOfWork.EnquiryRepository.Remove(enquiry);

            await _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error thrown in CleanUpData");
        }
    }
}
