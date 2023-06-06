using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry;
using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Logging;

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
    private const string EnquiryTuitionTypeKey = "enquiry_tuition_type";
    private const string EnquiryResponseTuitionTypeKey = "enquiry_response_tuition_type";
    private const string EnquiryTuitionPlanKey = "enquiry_tuition_plan";
    private const string EnquiryResponseTuitionPlanKey = "enquiry_response_tuition_plan";
    private const string EnquirySENDSupportKey = "enquiry_send_support";
    private const string EnquiryResponseSENDSupportKey = "enquiry_response_send_support";
    private const string EnquiryAdditionalInformationKey = "enquiry_additional_information";
    private const string EnquiryResponseAdditionalInformationKey = "enquiry_response_additional_information";
    private const string EnquiryTuitionPartnerNameKey = "tuition_partner_name";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<AddEnquiryResponseCommandHandler> _logger;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork, INotificationsClientService notificationsClientService,
        ILogger<AddEnquiryResponseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
        _logger = logger;
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

        request.Data.Email = tpEnquiry.Enquiry.Email;
        var enquirerToken = tpEnquiry.Enquiry.MagicLink.Token;

        tpEnquiry.EnquiryResponse = new EnquiryResponse()
        {
            TutoringLogisticsText = request.Data!.TutoringLogisticsText!,
            KeyStageAndSubjectsText = request.Data!.KeyStageAndSubjectsText!,
            TuitionTypeText = request.Data.TuitionTypeText!,
            SENDRequirementsText = request.Data.SENDRequirementsText ?? null,
            AdditionalInformationText = request.Data.AdditionalInformationText ?? null,
            CompletedAt = DateTime.UtcNow
        };

        var enquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient =
            GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(request,
                tpEnquiry.TuitionPartner.Name, tpEnquiry.Enquiry.SupportReferenceNumber, enquirerToken);

        var enquiryResponseSubmittedConfirmationToTpNotificationsRecipient =
            GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(
                request,
                tpEnquiry.TuitionPartner.Name,
                tpEnquiry.TuitionPartner.Email,
                tpEnquiry.Enquiry.SupportReferenceNumber,
                tpEnquiry.EnquiryResponse.CreatedAt);

        try
        {
            await _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to save the enquiry response");
            throw;
        }

        try
        {
            await _notificationsClientService.SendEmailAsync(
                enquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient,
                EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer);
        }
        catch { } //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in NotificationsClientService

        try
        {
            await _notificationsClientService.SendEmailAsync(
                enquiryResponseSubmittedConfirmationToTpNotificationsRecipient,
                EmailTemplateType.EnquiryResponseSubmittedConfirmationToTp);
        }
        catch { } //We suppress the exceptions here since we want the user to get the confirmation page, errors are logged in NotificationsClientService

        result.SupportReferenceNumber = tpEnquiry.Enquiry.SupportReferenceNumber;
        result.EnquirerMagicLink = enquirerToken;
        result.TuitionPartnerName = tpEnquiry.TuitionPartner.Name;

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

        if (string.IsNullOrWhiteSpace(request.Data.TuitionTypeText))
        {
            return "Data.TuitionTypeText is null or empty";
        }

        if (string.IsNullOrWhiteSpace(request.Data.TutoringLogisticsText))
        {
            return "Data.TutoringLogisticsText is null or empty";
        }

        return null;
    }

    private NotificationsRecipientDto GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string supportRefNumber, string enquirerToken)
    {
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/{supportRefNumber}?Token={enquirerToken}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            OriginalEmail = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(tpName, pageLink)
        };

        result.AddDefaultEnquiryDetails(
            supportRefNumber, request.Data!.BaseServiceUrl!, EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer,
            null, tpName);

        return result;
    }

    private static Dictionary<string, dynamic> GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(string tpName,
        string enquirerViewResponsesPageLinkKey)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, tpName },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewResponsesPageLinkKey }
        };

        return personalisation;
    }

    private NotificationsRecipientDto GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string tpEmail, string supportRefNumber,
        DateTime responseCreateDateTime)
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

        var personalisationInput = new EnquiryResponseToTpPersonalisationInput
        {
            TpName = tpName,
            LocalAreaDistrict = request.Data.LocalAuthorityDistrict,
            EnquiryKeyStageSubjects = $"{StringConstants.NotifyBulletedListFormat}{string.Join(Environment.NewLine + StringConstants.NotifyBulletedListFormat, request.Data.EnquiryKeyStageSubjects!)}",
            EnquiryResponseKeyStageSubjects = request.Data.KeyStageAndSubjectsText.EscapeNotifyText(true),
            EnquiryTuitionType = request.Data.EnquiryTuitionType,
            EnquiryResponseTuitionType = request.Data.TuitionTypeText.EscapeNotifyText(true),
            EnquiryTuitionPlan = enquiryTutoringLogistics,
            EnquiryResponseTuitionPlan = request.Data.TutoringLogisticsText.EscapeNotifyText(true),
            EnquirySendSupport = request.Data.EnquirySENDRequirements.EscapeNotifyText() ?? StringConstants.NotSpecified,
            EnquiryResponseSendSupport = request.Data.SENDRequirementsText.EscapeNotifyText(true) ?? StringConstants.NotSpecified,
            EnquiryAdditionalInformation = request.Data.EnquiryAdditionalInformation.EscapeNotifyText() ?? StringConstants.NotSpecified,
            EnquiryResponseAdditionalInformation =
                request.Data.AdditionalInformationText.EscapeNotifyText(true) ?? StringConstants.NotSpecified

        };

        var result = new NotificationsRecipientDto()
        {
            Email = tpEmail!,
            OriginalEmail = tpEmail!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetEnquiryResponseSubmittedConfirmationToTpPersonalisation(personalisationInput)
        };

        result.AddDefaultEnquiryDetails(
            supportRefNumber, request.Data!.BaseServiceUrl!, EmailTemplateType.EnquiryResponseSubmittedConfirmationToTp,
            responseCreateDateTime, tpName);

        return result;
    }

    private static Dictionary<string, dynamic>
        GetEnquiryResponseSubmittedConfirmationToTpPersonalisation(EnquiryResponseToTpPersonalisationInput input)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, input.TpName! },
            { EnquiryLadNameKey, input.LocalAreaDistrict! },
            { EnquiryKeyStageAndSubjects, input.EnquiryKeyStageSubjects! },
            { EnquiryResponseKeyStageAndSubjects, input.EnquiryResponseKeyStageSubjects! },
            { EnquiryTuitionTypeKey, input.EnquiryTuitionType! },
            { EnquiryResponseTuitionTypeKey, input.EnquiryResponseTuitionType! },
            { EnquiryTuitionPlanKey, input.EnquiryTuitionPlan! },
            { EnquiryResponseTuitionPlanKey, input.EnquiryResponseTuitionPlan! },
            { EnquirySENDSupportKey, input.EnquirySendSupport! },
            { EnquiryResponseSENDSupportKey, input.EnquiryResponseSendSupport! },
            { EnquiryAdditionalInformationKey, input.EnquiryAdditionalInformation! },
            { EnquiryResponseAdditionalInformationKey, input.EnquiryResponseAdditionalInformation! }
        };

        return personalisation;
    }

}