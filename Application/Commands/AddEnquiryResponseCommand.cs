using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
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
    private const string EnquiryTextVariableKey = "enquiry";
    private const string EnquiryResponderVariableKey = "enquiry_responder";
    private const string EnquiryResponseTextVariableKey = "enquiry_response";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";

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

        var notificationsRecipient = GetNotificationsRecipient(request, enquirerEnquiryResponseReceivedData.TuitionPartnerName);

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
            TutoringLogisticsText = request.Data!.TutoringLogisticsText,
            KeyStageAndSubjectsText = request.Data.KeyStageAndSubjectsText,
            TuitionTypeText = request.Data.TuitionTypeText,
            SENDRequirementsText = request.Data.SENDRequirementsText ?? null,
            AdditionalInformationText = request.Data.AdditionalInformationText ?? null
        };

        tpEnquiry.MagicLinkId = magicLink?.Id;

        _unitOfWork.MagicLinkRepository.AddAsync(enquirerViewResponseMagicLink, cancellationToken);

        try
        {
            await _unitOfWork.Complete();

            await _notificationsClientService.SendEmailAsync(
                notificationsRecipient,
                EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer, tpEnquiry.Enquiry.SupportReferenceNumber);

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

    private NotificationsRecipientDto GetNotificationsRecipient(AddEnquiryResponseCommand request, string enquiryResponderText)
    {
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/respond/all-enquirer-responses?token={request.Data?.Token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetPersonalisation(request.Data?.TutoringLogisticsText!, request.Data?.TutoringLogisticsText!, enquiryResponderText, pageLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string enquiryResponseText,
        string enquiryResponderText,
        string enquirerViewResponsesPageLinkKey)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquiryResponderVariableKey, enquiryResponderText },
            { EnquiryResponseTextVariableKey, enquiryResponseText },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewResponsesPageLinkKey }
        };

        return personalisation;
    }
}