using System;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry;
using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmailStatus = Domain.Enums.EmailStatus;

namespace Application.Commands.Enquiry.Respond;

public record AddEnquiryResponseCommand : IRequest<ResponseConfirmationModel>
{
    public EnquiryResponseBaseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, ResponseConfirmationModel>
{
    private const string EnquiryLadNameKey = "local_area_district";
    private const string EnquiryKeyStageAndSubjects = "enquiry_keystage_subjects";
    private const string EnquiryResponseKeyStageAndSubjects = "enquiry_response_keystage_subjects";
    private const string EnquiryTuitionSettingKey = "enquiry_tuition_setting";
    private const string EnquiryResponseTuitionSettingKey = "enquiry_response_tuition_setting";
    private const string EnquiryTuitionPlanKey = "enquiry_tuition_plan";
    private const string EnquiryResponseTuitionPlanKey = "enquiry_response_tuition_plan";
    private const string EnquirySENDSupportKey = "enquiry_send_support";
    private const string EnquiryResponseSENDSupportKey = "enquiry_response_send_support";
    private const string EnquiryAdditionalInformationKey = "enquiry_additional_information";
    private const string EnquiryResponseAdditionalInformationKey = "enquiry_response_additional_information";
    private const string EnquiryTuitionPartnerNameKey = "tuition_partner_name";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IProcessEmailsService _processEmailsService;
    private readonly ILogger<AddEnquiryResponseCommandHandler> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    private readonly DateTime _createdDateTime = DateTime.UtcNow;
    private string? _environmentNameNonProduction;
    private string? _enquiryReferenceNumber;
    private string? _enquirerToken;
    private string? _enquirerEmail;
    private string? _tpName;
    private string? _tpEmail;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork,
        IProcessEmailsService processEmailsService,
        ILogger<AddEnquiryResponseCommandHandler> logger,
        IHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _processEmailsService = processEmailsService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<ResponseConfirmationModel> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var result = new ResponseConfirmationModel();

        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            validationResult = $"The {nameof(AddEnquiryResponseCommand)} {validationResult}";
            _logger.LogError(validationResult);
            throw new ArgumentException(validationResult);
        }

        var tpEnquiry = await _unitOfWork.TuitionPartnerEnquiryRepository
            .SingleOrDefaultAsync(x => x.Enquiry.SupportReferenceNumber == request.Data.SupportReferenceNumber &&
                                       x.TuitionPartner.SeoUrl == request.Data.TuitionPartnerSeoUrl, "Enquiry,Enquiry.MagicLink,TuitionPartner,EnquiryResponse",
                true, cancellationToken);

        if (tpEnquiry == null)
        {
            var errorMessage = $"Unable to find TuitionPartnerEnquiry with Support Ref ('{request.Data.SupportReferenceNumber}') and Tuition Partner SeoUrl ('{request.Data.TuitionPartnerSeoUrl}')";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        _environmentNameNonProduction = (_hostEnvironment.IsProduction() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null) ? string.Empty : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.ToString();
        _enquiryReferenceNumber = request.Data.SupportReferenceNumber;
        _enquirerToken = tpEnquiry.Enquiry.MagicLink.Token;
        _tpName = tpEnquiry.TuitionPartner.Name;
        _enquirerEmail = tpEnquiry.Enquiry.Email;
        _tpEmail = tpEnquiry.TuitionPartner.Email;

        tpEnquiry.EnquiryResponse = GetEnquiryResponse(request);

        try
        {
            await _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to save the enquiry response");
            throw;
        }

        await ProcessEmailsAsync(tpEnquiry.EnquiryResponse);

        result.SupportReferenceNumber = _enquiryReferenceNumber;
        result.EnquirerMagicLink = _enquirerToken;
        result.TuitionPartnerName = _tpName;

        return result;
    }


    private static string? ValidateRequest(AddEnquiryResponseCommand request)
    {
        if (request.Data == null)
        {
            return "Data is null";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TuitionPartnerSeoUrl))
        {
            return "Data.TuitionPartnerSeoUrl is missing";
        }

        if (string.IsNullOrWhiteSpace(request.Data.SupportReferenceNumber))
        {
            return "Data.SupportReferenceNumber is missing";
        }

        if (string.IsNullOrWhiteSpace(request.Data.KeyStageAndSubjectsText))
        {
            return "Data.KeyStageAndSubjectsText is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TuitionSettingText))
        {
            return "Data.TuitionSettingText is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsText))
        {
            return "Data.TutoringLogisticsText is null or empty";
        }

        return null;
    }

    private EnquiryResponse GetEnquiryResponse(AddEnquiryResponseCommand request)
    {
        return new EnquiryResponse()
        {
            TutoringLogisticsText = request.Data!.TutoringLogisticsText!,
            KeyStageAndSubjectsText = request.Data!.KeyStageAndSubjectsText!,
            TuitionSettingText = request.Data.TuitionSettingText!,
            SENDRequirementsText = request.Data.SENDRequirementsText ?? null,
            AdditionalInformationText = request.Data.AdditionalInformationText ?? null,
            CompletedAt = _createdDateTime,
            EnquirerResponseEmailLog = GetEnquirerResponseEmailLog(request),
            TuitionPartnerResponseEmailLog = GetTuitionPartnerResponseEmailLog(request)
        };
    }

    private EmailLog GetEnquirerResponseEmailLog(AddEnquiryResponseCommand request)
    {
        var emailTemplateType = EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer;

        var enquirerResponseEmailPersonalisationLog = GetEnquirerResponseEmailPersonalisationLog(request);

        var emailLog = new EmailLog()
        {
            CreatedDate = _createdDateTime,
            ProcessFromDate = _createdDateTime,
            FinishProcessingDate = _createdDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = _enquirerEmail!,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(_enquirerEmail!),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = _enquiryReferenceNumber!.CreateNotifyEnquiryClientReference(_environmentNameNonProduction!, emailTemplateType, _tpName!),
            EmailStatusId = (int)EmailStatus.ToBeProcessed,
            EmailPersonalisationLogs = enquirerResponseEmailPersonalisationLog
        };

        return emailLog;
    }

    private List<EmailPersonalisationLog> GetEnquirerResponseEmailPersonalisationLog(AddEnquiryResponseCommand request)
    {
        var enquirerViewAllResponsesPageLinkKey = $"{request.Data?.BaseServiceUrl}/enquiry/{_enquiryReferenceNumber}?Token={_enquirerToken}";

        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, _tpName! },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLinkKey }
        };

        personalisation.AddDefaultEnquiryPersonalisation(_enquiryReferenceNumber!, request.Data!.BaseServiceUrl!);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }

    private EmailLog GetTuitionPartnerResponseEmailLog(AddEnquiryResponseCommand request)
    {
        var emailTemplateType = EmailTemplateType.EnquiryResponseSubmittedConfirmationToTp;

        var tuitionPartnerResponseEmailPersonalisationLog = GetTuitionPartnerResponseEmailPersonalisationLog(request);

        var emailLog = new EmailLog()
        {
            CreatedDate = _createdDateTime,
            ProcessFromDate = _createdDateTime,
            FinishProcessingDate = _createdDateTime.AddDays(IntegerConstants.EmailLogFinishProcessingAfterDays),
            EmailAddress = _tpEmail!,
            EmailAddressUsedForTesting = _processEmailsService.GetEmailAddressUsedForTesting(_enquirerEmail!),
            EmailTemplateShortName = emailTemplateType.DisplayName(),
            ClientReferenceNumber = _enquiryReferenceNumber!.CreateNotifyEnquiryClientReference(_environmentNameNonProduction!, emailTemplateType, _tpName!),
            EmailStatusId = (int)EmailStatus.ToBeProcessed,
            EmailPersonalisationLogs = tuitionPartnerResponseEmailPersonalisationLog
        };

        return emailLog;
    }

    private List<EmailPersonalisationLog> GetTuitionPartnerResponseEmailPersonalisationLog(AddEnquiryResponseCommand request)
    {
        string enquiryTutoringLogistics;
        if (request.Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel != null)
        {
            var detailsModel = request.Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel;
            enquiryTutoringLogistics = $"{StringConstants.NotifyBulletedListFormat}Number of pupils: {detailsModel!.NumberOfPupils.EscapeNotifyText()}{Environment.NewLine}" +
                        $"{StringConstants.NotifyBulletedListFormat}Start date: {detailsModel!.StartDate.EscapeNotifyText()}{Environment.NewLine}" +
                        $"{StringConstants.NotifyBulletedListFormat}Tuition duration: {detailsModel!.TuitionDuration.EscapeNotifyText()}{Environment.NewLine}" +
                        $"{StringConstants.NotifyBulletedListFormat}Time of day: {detailsModel!.TimeOfDay.EscapeNotifyText()}";
        }
        else
        {
            enquiryTutoringLogistics = request.Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics.EscapeNotifyText()!;
        }

        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, _tpName! },
            { EnquiryLadNameKey, request.Data.LocalAuthorityDistrict },
            { EnquiryKeyStageAndSubjects, $"{StringConstants.NotifyBulletedListFormat}{string.Join(Environment.NewLine + StringConstants.NotifyBulletedListFormat, request.Data.EnquiryKeyStageSubjects!)}" },
            { EnquiryResponseKeyStageAndSubjects, request.Data.KeyStageAndSubjectsText.EscapeNotifyText(true)! },
            { EnquiryTuitionSettingKey, request.Data.EnquiryTuitionSetting },
            { EnquiryResponseTuitionSettingKey, request.Data.TuitionSettingText.EscapeNotifyText(true)! },
            { EnquiryTuitionPlanKey, enquiryTutoringLogistics },
            { EnquiryResponseTuitionPlanKey, request.Data.TutoringLogisticsText.EscapeNotifyText(true)! },
            { EnquirySENDSupportKey, request.Data.EnquirySENDRequirements.EscapeNotifyText() ?? StringConstants.NotSpecified },
            { EnquiryResponseSENDSupportKey, request.Data.SENDRequirementsText.EscapeNotifyText(true) ?? StringConstants.NotSpecified },
            { EnquiryAdditionalInformationKey, request.Data.EnquiryAdditionalInformation.EscapeNotifyText() ?? StringConstants.NotSpecified },
            { EnquiryResponseAdditionalInformationKey, request.Data.AdditionalInformationText.EscapeNotifyText(true) ?? StringConstants.NotSpecified }
        };

        personalisation.AddDefaultEnquiryPersonalisation(_enquiryReferenceNumber!, request.Data!.BaseServiceUrl!, _createdDateTime!);

        return personalisation.Select(x => new EmailPersonalisationLog
        {
            Key = x.Key,
            Value = x.Value.ToString()
        }).ToList();
    }

    private async Task ProcessEmailsAsync(EnquiryResponse enquiryResponse)
    {
        try
        {
            await _processEmailsService.SendEmailAsync(enquiryResponse.EnquirerResponseEmailLogId);
        }
        catch
        {
            //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in the services
        }

        try
        {
            await _processEmailsService.SendEmailAsync(enquiryResponse.TuitionPartnerResponseEmailLogId);
        }
        catch
        {
            //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in the services
        }
    }
}