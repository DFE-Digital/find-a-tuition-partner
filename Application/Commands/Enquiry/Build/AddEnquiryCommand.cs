using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Domain.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly INotificationsClientService _notificationsClientService;
    private readonly IGenerateReferenceNumber _generateReferenceNumber;
    private readonly ILogger<AddEnquiryCommandHandler> _logger;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork, IRandomTokenGenerator randomTokenGenerator,
        INotificationsClientService notificationsClientService,
        IGenerateReferenceNumber generateReferenceNumber, ILogger<AddEnquiryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _randomTokenGenerator = randomTokenGenerator;
        _notificationsClientService = notificationsClientService;
        _generateReferenceNumber = generateReferenceNumber;
        _logger = logger;
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

        var enquirySubmittedToTpNotificationsRecipients = GetEnquirySubmittedToTpNotificationsRecipients(
            request.Data!.TuitionPartnersForEnquiry!.Results, request.Data?.Email!);

        var enquirySubmittedConfirmationToEnquirerNotificationsRecipient =
            GetEnquirySubmittedConfirmationToEnquirerNotificationsRecipient(request);

        var enquirerMagicLink = new MagicLink()
        {
            Token = enquirySubmittedConfirmationToEnquirerNotificationsRecipient.Token!
        };

        var tuitionPartnerEnquiry = enquirySubmittedToTpNotificationsRecipients.Select(selectedTuitionPartner =>
            new TuitionPartnerEnquiry()
            {
                TuitionPartnerId = selectedTuitionPartner.TuitionPartnerId,
                MagicLink = new MagicLink()
                {
                    Token = selectedTuitionPartner.Token!
                },
                ResponseCloseDate = DateTime.UtcNow.AddDays(IntegerConstants.EnquiryDaysToRespond)
            }).ToList();

        var tuitionTypeId = GetTuitionTypeId(request.Data!.TuitionType);

        var enquiry = new Domain.Enquiry()
        {
            Email = request.Data!.Email!,
            TutoringLogistics = request.Data!.TutoringLogisticsDetailsModel.ToJson(),
            SENDRequirements = request.Data!.SENDRequirements ?? null,
            AdditionalInformation = request.Data!.AdditionalInformation ?? null,
            TuitionPartnerEnquiry = tuitionPartnerEnquiry,
            SupportReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber(),
            KeyStageSubjectEnquiry = GetKeyStageSubjectsEnquiry(request.Data!.Subjects!.ParseKeyStageSubjects()),
            PostCode = request.Data!.Postcode!,
            LocalAuthorityDistrict = request.Data!.TuitionPartnersForEnquiry!.LocalAuthorityDistrictName!,
            TuitionTypeId = tuitionTypeId,
            MagicLink = enquirerMagicLink
        };

        try
        {
            _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

            await _unitOfWork.Complete();
        }
        catch (DbUpdateException ex)
        {
            await HandleDbUpdateException(ex, enquiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to save the enquiry.");
            throw;
        }

        _logger.LogInformation("Enquiry successfully created with magic links. EnquiryId: {enquiryId}", enquiry.Id);

        var enquirerViewAllResponsesPageLink =
            $"{request.Data?.BaseServiceUrl}/enquiry/{enquiry.SupportReferenceNumber}?Token={enquirerMagicLink.Token}";

        enquirySubmittedConfirmationToEnquirerNotificationsRecipient.Personalisation = GetGetEnquirySubmittedConfirmationToEnquirerPersonalisation(
                    request.Data!.TuitionPartnersForEnquiry!.Results!.Count(), enquirerViewAllResponsesPageLink);

        enquirySubmittedConfirmationToEnquirerNotificationsRecipient.AddDefaultEnquiryDetails(
            enquiry.SupportReferenceNumber, request.Data!.BaseServiceUrl!, EmailTemplateType.EnquirySubmittedConfirmationToEnquirer,
            enquiry.CreatedAt);

        await TrySendEnquirySubmittedConfirmationToEnquirerEmail(enquiry,
            enquirySubmittedConfirmationToEnquirerNotificationsRecipient);

        enquirySubmittedToTpNotificationsRecipients.ForEach(x =>
            x.Personalisation = GetEnquirySubmittedToTpPersonalisation(x.TuitionPartnerName!,
                $"{request.Data!.BaseServiceUrl}/enquiry-response/{x.TuitionPartnerName.ToSeoUrl()}/{enquiry.SupportReferenceNumber}?Token={x.Token}",
                request!.Data!.TuitionPartnersForEnquiry!.LocalAuthorityDistrictName!
                ));

        enquirySubmittedToTpNotificationsRecipients.ForEach(x =>
            x.AddDefaultEnquiryDetails(
            enquiry.SupportReferenceNumber, request.Data!.BaseServiceUrl!, EmailTemplateType.EnquirySubmittedToTp,
            enquiry.CreatedAt, x.TuitionPartnerName));

        try
        {
            await _notificationsClientService.SendEmailAsync(enquirySubmittedToTpNotificationsRecipients,
            EmailTemplateType.EnquirySubmittedToTp);
        }
        catch { } //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in NotificationsClientService

        var tpMagicLinkModelList = new List<TuitionPartnerMagicLinkModel>();

        result.SupportReferenceNumber = enquiry.SupportReferenceNumber;
        result.EnquirerMagicLink = enquirySubmittedConfirmationToEnquirerNotificationsRecipient.Token;
        enquirySubmittedToTpNotificationsRecipients.ForEach(x =>
            tpMagicLinkModelList.Add(new TuitionPartnerMagicLinkModel()
            {
                TuitionPartnerSeoUrl = x.TuitionPartnerName.ToSeoUrl()!,
                Email = x.OriginalEmail,
                MagicLinkToken = x.Token!
            }));

        result.TuitionPartnerMagicLinks = tpMagicLinkModelList;

        result.TuitionPartnerMagicLinksCount = tpMagicLinkModelList.Count;

        return result;
    }

    private List<NotificationsRecipientDto> GetEnquirySubmittedToTpNotificationsRecipients(
        IEnumerable<TuitionPartnerResult> recipients, string enquirerEmailForTestingPurposes)
    {
        return (from recipient in recipients
                let token = _randomTokenGenerator.GenerateRandomToken()
                select new NotificationsRecipientDto()
                {
                    Email = recipient.Email,
                    OriginalEmail = recipient.Email,
                    EnquirerEmailForTestingPurposes = enquirerEmailForTestingPurposes,
                    Token = token,
                    TuitionPartnerId = recipient.Id,
                    TuitionPartnerName = recipient.Name,
                    PersonalisationPropertiesToAmalgamate = new List<string>()
                    { EnquiryTpNameKey, EnquiryResponseFormLinkKey }
                }).ToList();
    }

    private static Dictionary<string, dynamic> GetEnquirySubmittedToTpPersonalisation(string tpName,
        string responseFormLink,
        string ladNameKey)
    {
        var personalisation = new Dictionary<string, dynamic>()
            {
                { EnquiryTpNameKey, tpName },
                { EnquiryResponseFormLinkKey, responseFormLink },
                { EnquiryLadNameKey, ladNameKey },
                { EnquiryDaysToRespond, IntegerConstants.EnquiryDaysToRespond }
            };

        return personalisation;
    }

    private NotificationsRecipientDto GetEnquirySubmittedConfirmationToEnquirerNotificationsRecipient(
        AddEnquiryCommand request)
    {
        var token = _randomTokenGenerator.GenerateRandomToken();

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            OriginalEmail = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Token = token
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetGetEnquirySubmittedConfirmationToEnquirerPersonalisation(
        int numberOfTpsContacted,
        string enquirerViewAllResponsesPageLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
            {
                { EnquiryNumberOfTpsContactedKey, numberOfTpsContacted.ToString() },
                { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLink },
                { EnquiryDaysToRespond, IntegerConstants.EnquiryDaysToRespond }
            };

        return personalisation;
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

        if (string.IsNullOrWhiteSpace(request.Data.Email))
        {
            return "Data.Email is null or empty";
        }

        if (request.Data.TutoringLogisticsDetailsModel == null)
        {
            return "Data.TutoringLogisticsDetailsModel is null";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsDetailsModel.NumberOfPupils))
        {
            return "Data.TutoringLogisticsDetailsModel.NumberOfPupils is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsDetailsModel.StartDate))
        {
            return "Data.TutoringLogisticsDetailsModel.StartDate is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsDetailsModel.TuitionDuration))
        {
            return "Data.TutoringLogisticsDetailsModel.TuitionDuration is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsDetailsModel.TimeOfDay))
        {
            return "Data.TutoringLogisticsDetailsModel.TimeOfDay is null or empty";
        }

        return null;
    }

    private async Task HandleDbUpdateException(DbUpdateException ex, Domain.Enquiry enquiry)
    {
        var dataSaved = false;
        var retryAttempt = 0;

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
                    enquiry.SupportReferenceNumber, retryAttempt);

                enquiry.SupportReferenceNumber = _generateReferenceNumber.GenerateReferenceNumber();

                _logger.LogInformation("Generating new support reference number: {referenceNumber}",
                    enquiry.SupportReferenceNumber);

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
    }

    private async Task TrySendEnquirySubmittedConfirmationToEnquirerEmail(Domain.Enquiry enquiry,
        NotificationsRecipientDto getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient)
    {
        try
        {
            await _notificationsClientService.SendEmailAsync(
                getEnquirySubmittedConfirmationToEnquirerNotificationsRecipient,
                EmailTemplateType.EnquirySubmittedConfirmationToEnquirer);
        }
        catch (Exception)
        {
            await CleanUpData(enquiry);
            throw;
        }
    }

    private async Task CleanUpData(Domain.Enquiry enquiry)
    {
        _unitOfWork.EnquiryRepository.Remove(enquiry);
        var tpMagicLinks = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetAllAsync(x => x.Enquiry.SupportReferenceNumber == enquiry.SupportReferenceNumber, "Enquiry,MagicLink",
                true);
        tpMagicLinks.Select(x => x.MagicLink).ToList().ForEach(x => _unitOfWork.MagicLinkRepository.Remove(x));
        _unitOfWork.MagicLinkRepository.Remove(enquiry.MagicLink);
        await _unitOfWork.Complete();
    }
}
