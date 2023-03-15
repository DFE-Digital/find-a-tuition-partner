using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Application.Commands;

public record AddEnquiryResponseCommand : IRequest<string>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, string>
{
    private const string EnquiryReferenceNumberKey = "enquiry_ref_number";
    private const string EnquiryLadNameKey = "local_area_district";
    private const string EnquiryCreatedDateTime = "date_time";
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
    private const string ContactUsLink = "contact_us_link";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IEncrypt _aesEncryption;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<AddEnquiryResponseCommandHandler> _logger;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork, INotificationsClientService notificationsClientService,
        IEncrypt aesEncryption, ILogger<AddEnquiryResponseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
        _aesEncryption = aesEncryption;
        _logger = logger;
    }

    public async Task<string> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var emptyResult = string.Empty;

        var enquiryId = request.Data.EnquiryId;

        var tuitionPartnerId = request.Data.TuitionPartnerId;

        if (string.IsNullOrEmpty(request.Data.Token))
        {
            return emptyResult;
        }

        var magicLink =
            await _unitOfWork.MagicLinkRepository
                .SingleOrDefaultAsync(x => x.Token == request.Data.Token, null, true, cancellationToken);

        if (enquiryId == default || tuitionPartnerId == default) return emptyResult;

        var tpEnquiry = await _unitOfWork.TuitionPartnerEnquiryRepository
            .SingleOrDefaultAsync(x => x.EnquiryId == enquiryId &&
                                       x.TuitionPartnerId == tuitionPartnerId, "Enquiry,TuitionPartner,EnquiryResponse",
                true, cancellationToken);

        if (tpEnquiry == null)
        {
            _logger.LogError("Unable to find TuitionPartnerEnquiry with the enquiry Id {enquiryId} and tuition partnerId {tuitionPartnerId}",
                enquiryId, tuitionPartnerId);
            return emptyResult;
        }

        var enquirerEnquiryResponseReceivedData =
            await _unitOfWork.MagicLinkRepository.
                GetEnquirerEnquiryResponseReceivedData(request.Data.EnquiryId!, request.Data.TuitionPartnerId);

        if (enquirerEnquiryResponseReceivedData == null)
        {
            _logger.LogError("Unable to send enquirer enquiry response received email. " +
                             "Can't find magic link token with the type {type} by enquiry Id {enquiryId}",
                MagicLinkType.EnquirerViewAllResponses.ToString(), request.Data.EnquiryId);

            return emptyResult;
        }

        request.Data.Token = enquirerEnquiryResponseReceivedData.Token!;
        request.Data.Email = enquirerEnquiryResponseReceivedData.Email!;
        request.Data.TutoringLogisticsText = enquirerEnquiryResponseReceivedData.TutoringLogisticsText!;

        var contactUsLink = $"{request.Data.BaseServiceUrl}/contact-us";

        var enquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient =
            GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(request,
            enquirerEnquiryResponseReceivedData.TuitionPartnerName, contactUsLink);

        var enquiryResponseSubmittedConfirmationToTpNotificationsRecipient =
            GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(
                request,
                tpEnquiry.TuitionPartner.Name,
                tpEnquiry.Enquiry.SupportReferenceNumber,
                contactUsLink,
                tpEnquiry.Enquiry.CreatedAt);

        GenerateEnquirerViewResponseToken(request, out var enquirerViewResponseMagicLinkToken);
        request.Data.TutoringLogisticsText = tpEnquiry.Enquiry.TutoringLogistics;

        var enquirerViewResponseMagicLink = new MagicLink()
        {
            Token = enquirerViewResponseMagicLinkToken,
            EnquiryId = request.Data?.EnquiryId,
            MagicLinkTypeId = (int)MagicLinkType.EnquirerViewResponse
        };

        tpEnquiry.EnquiryResponse = new EnquiryResponse()
        {
            EnquiryId = enquiryId,
            MagicLink = enquirerViewResponseMagicLink,
            TutoringLogisticsText = request.Data!.TutoringLogisticsText!,
            KeyStageAndSubjectsText = request.Data!.KeyStageAndSubjectsText!,
            TuitionTypeText = request.Data.TuitionTypeText!,
            SENDRequirementsText = request.Data.SENDRequirementsText ?? null,
            AdditionalInformationText = request.Data.AdditionalInformationText ?? null
        };

        tpEnquiry.MagicLinkId = magicLink?.Id;

        _unitOfWork.MagicLinkRepository.AddAsync(enquirerViewResponseMagicLink, cancellationToken);

        try
        {
            await _unitOfWork.Complete();

            await _notificationsClientService.SendEmailAsync(
                enquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient,
                EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer, tpEnquiry.Enquiry.SupportReferenceNumber);

            await _notificationsClientService.SendEmailAsync(
                enquiryResponseSubmittedConfirmationToTpNotificationsRecipient,
            EmailTemplateType.EnquiryResponseSubmittedConfirmationToTp,
                tpEnquiry.Enquiry.SupportReferenceNumber
            );

            return tpEnquiry.Enquiry.SupportReferenceNumber;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while trying to save the enquiry response. Error: {ex}", ex);
        }

        return emptyResult;
    }

    private void GenerateEnquirerViewResponseToken(AddEnquiryResponseCommand request,
        out string enquirerViewResponseMagicLinkToken)
    {
        var generateRandomness
            = _aesEncryption.GenerateRandomToken();
        enquirerViewResponseMagicLinkToken = _aesEncryption.Encrypt(
            $"EnquiryId={request.Data?.EnquiryId}&TuitionPartnerId={request.Data!.TuitionPartnerId}&Type={nameof(MagicLinkType.EnquirerViewResponse)}&{generateRandomness}");
    }

    private NotificationsRecipientDto GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string contactusLink)
    {
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/respond/all-enquirer-responses?token={request.Data?.Token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(tpName, pageLink, contactusLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(string tpName,
        string enquirerViewResponsesPageLinkKey, string contactusLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, tpName },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewResponsesPageLinkKey },
            { ContactUsLink, contactusLink }
        };

        return personalisation;
    }

    private NotificationsRecipientDto GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string supportRefNumber, string contactusLink, DateTime enquiryCreateDateTime)
    {
        var personalisationInput = new EnquiryResponseToTpPersonalisationInput
        {
            TpName = tpName,
            SupportRefNumber = supportRefNumber,
            LocalAreaDistrict = request.Data.LocalAuthorityDistrict,
            CreatedOnDateTime = enquiryCreateDateTime.ToString("dd/MM/yyyy HH:mm"),
            EnquiryKeyStageSubjects = string.Join(Environment.NewLine, request.Data.EnquiryKeyStageSubjects!),
            EnquiryResponseKeyStageSubjects = request.Data.KeyStageAndSubjectsText,
            EnquiryTuitionType = request.Data.EnquiryTuitionType,
            EnquiryResponseTuitionType = request.Data.TuitionTypeText,
            EnquiryTuitionPlan = request.Data.EnquiryTutoringLogistics,
            EnquiryResponseTuitionPlan = request.Data.TutoringLogisticsText,
            EnquirySendSupport = request.Data.EnquirySENDRequirements ?? StringConstants.NotSpecified,
            EnquiryResponseSendSupport = request.Data.SENDRequirementsText ?? StringConstants.NotSpecified,
            EnquiryAdditionalInformation = request.Data.EnquiryAdditionalInformation ?? StringConstants.NotSpecified,
            EnquiryResponseAdditionalInformation =
                request.Data.AdditionalInformationText ?? StringConstants.NotSpecified,
            ContactUsLink = contactusLink
        };

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetEnquiryResponseSubmittedConfirmationToTpPersonalisation(personalisationInput)
        };
        return result;
    }

    private static Dictionary<string, dynamic>
        GetEnquiryResponseSubmittedConfirmationToTpPersonalisation(EnquiryResponseToTpPersonalisationInput input)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTuitionPartnerNameKey, input.TpName! },
            { EnquiryReferenceNumberKey, input.SupportRefNumber! },
            { EnquiryLadNameKey, input.LocalAreaDistrict! },
            { EnquiryCreatedDateTime, input.CreatedOnDateTime! },
            { EnquiryKeyStageAndSubjects, input.EnquiryKeyStageSubjects! },
            { EnquiryResponseKeyStageAndSubjects, input.EnquiryResponseKeyStageSubjects! },
            { EnquiryTuitionTypeKey, input.EnquiryTuitionType! },
            { EnquiryResponseTuitionTypeKey, input.EnquiryResponseTuitionType! },
            { EnquiryTuitionPlanKey, input.EnquiryTuitionPlan! },
            { EnquiryResponseTuitionPlanKey, input.EnquiryResponseTuitionPlan! },
            { EnquirySENDSupportKey, input.EnquirySendSupport! },
            { EnquiryResponseSENDSupportKey, input.EnquiryResponseSendSupport! },
            { EnquiryAdditionalInformationKey, input.EnquiryAdditionalInformation! },
            { EnquiryResponseAdditionalInformationKey, input.EnquiryResponseAdditionalInformation! },
            { ContactUsLink, input.ContactUsLink! }
        };

        return personalisation;
    }

}