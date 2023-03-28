using System.Net;
using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public record AddEnquiryResponseCommand : IRequest<SubmittedConfirmationModel>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, SubmittedConfirmationModel>
{
    private const string EnquiryReferenceNumberKey = "enquiry_ref_number";
    private const string EnquiryLadNameKey = "local_area_district";
    private const string EnquiryResponseCreatedDateTime = "date_time";
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

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<AddEnquiryResponseCommandHandler> _logger;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork, INotificationsClientService notificationsClientService,
        ILogger<AddEnquiryResponseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
        _logger = logger;
    }

    public async Task<SubmittedConfirmationModel> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var result = new SubmittedConfirmationModel();

        if (string.IsNullOrEmpty(request.Data.Token))
        {
            result.IsValid = false;
            result.ErrorStatus = HttpStatusCode.NotFound.ToString();
            return result;
        }

        var tpEnquiry = await _unitOfWork.TuitionPartnerEnquiryRepository
            .SingleOrDefaultAsync(x => x.Enquiry.SupportReferenceNumber == request.Data.SupportReferenceNumber &&
                                       x.MagicLink!.Token == request.Data.Token, "Enquiry,TuitionPartner,EnquiryResponse",
                true, cancellationToken);

        if (tpEnquiry == null)
        {
            _logger.LogError("Unable to find TuitionPartnerEnquiry with the SupportReferenceNumber {supportReferenceNumber} and Token {token}",
                request.Data.SupportReferenceNumber, request.Data.Token);
            result.IsValid = false;
            result.ErrorStatus = HttpStatusCode.NotFound.ToString();
            return result;
        }

        request.Data.Email = tpEnquiry.Enquiry.Email;

        tpEnquiry.EnquiryResponse = new EnquiryResponse()
        {
            TutoringLogisticsText = request.Data!.TutoringLogisticsText!,
            KeyStageAndSubjectsText = request.Data!.KeyStageAndSubjectsText!,
            TuitionTypeText = request.Data.TuitionTypeText!,
            SENDRequirementsText = request.Data.SENDRequirementsText ?? null,
            AdditionalInformationText = request.Data.AdditionalInformationText ?? null,
            CompletedAt = DateTime.UtcNow
        };

        var contactUsLink = $"{request.Data.BaseServiceUrl}/contact-us";

        var enquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient =
            GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(request,
                tpEnquiry.TuitionPartner.Name, tpEnquiry.Enquiry.SupportReferenceNumber, contactUsLink);

        var enquiryResponseSubmittedConfirmationToTpNotificationsRecipient =
            GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(
                request,
                tpEnquiry.TuitionPartner.Name,
                tpEnquiry.TuitionPartner.Email,
                tpEnquiry.Enquiry.SupportReferenceNumber,
                contactUsLink,
                tpEnquiry.EnquiryResponse.CreatedAt);

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
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while trying to save the enquiry response. Error: {ex}", ex);
            result.IsValid = false;
            result.ErrorStatus = HttpStatusCode.InternalServerError.ToString();
            return result;
        }

        result.SupportReferenceNumber = tpEnquiry.Enquiry.SupportReferenceNumber;
        result.EnquirerMagicLink = request.Data?.Token;
        result.TuitionPartnerName = tpEnquiry.TuitionPartner.Name;

        return result;
    }

    private NotificationsRecipientDto GetEnquiryResponseReceivedConfirmationToEnquirerNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string supportRefNumber, string contactusLink)
    {
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/{supportRefNumber}?Token={request.Data?.Token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            OriginalEmail = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(tpName, supportRefNumber, pageLink, contactusLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetEnquiryResponseReceivedConfirmationToEnquirerPersonalisation(string tpName,
        string supportRefNumber, string enquirerViewResponsesPageLinkKey, string contactusLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryReferenceNumberKey, supportRefNumber},
            { EnquiryTuitionPartnerNameKey, tpName },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewResponsesPageLinkKey },
            { ContactUsLink, contactusLink }
        };

        return personalisation;
    }

    private NotificationsRecipientDto GetEnquiryResponseSubmittedConfirmationToTpNotificationsRecipient(AddEnquiryResponseCommand request,
        string tpName, string tpEmail, string supportRefNumber, string contactusLink, DateTime responseCreateDateTime)
    {
        var personalisationInput = new EnquiryResponseToTpPersonalisationInput
        {
            TpName = tpName,
            SupportRefNumber = supportRefNumber,
            LocalAreaDistrict = request.Data.LocalAuthorityDistrict,
            ResponseCreatedOnDateTime = responseCreateDateTime.ToLocalDateTime().ToString(StringConstants.DateFormatGDS),
            EnquiryKeyStageSubjects = string.Join(Environment.NewLine, request.Data.EnquiryKeyStageSubjects!),
            EnquiryResponseKeyStageSubjects = request.Data.KeyStageAndSubjectsText.EscapeNotifyText(true),
            EnquiryTuitionType = request.Data.EnquiryTuitionType,
            EnquiryResponseTuitionType = request.Data.TuitionTypeText.EscapeNotifyText(true),
            EnquiryTuitionPlan = request.Data.EnquiryTutoringLogistics.EscapeNotifyText(),
            EnquiryResponseTuitionPlan = request.Data.TutoringLogisticsText.EscapeNotifyText(true),
            EnquirySendSupport = request.Data.EnquirySENDRequirements.EscapeNotifyText() ?? StringConstants.NotSpecified,
            EnquiryResponseSendSupport = request.Data.SENDRequirementsText.EscapeNotifyText(true) ?? StringConstants.NotSpecified,
            EnquiryAdditionalInformation = request.Data.EnquiryAdditionalInformation.EscapeNotifyText() ?? StringConstants.NotSpecified,
            EnquiryResponseAdditionalInformation =
                request.Data.AdditionalInformationText.EscapeNotifyText(true) ?? StringConstants.NotSpecified,
            ContactUsLink = contactusLink
        };

        var result = new NotificationsRecipientDto()
        {
            Email = tpEmail!,
            OriginalEmail = tpEmail!,
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
            { EnquiryResponseCreatedDateTime, input.ResponseCreatedOnDateTime! },
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