using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Domain;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Application.Commands;

public record AddEnquiryResponseCommand : IRequest<bool>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, bool>
{
    private const string EnquiryTextVariableKey = "enquiry";
    private const string EnquiryResponderVariableKey = "enquiry_responder";
    private const string EnquiryResponseTextVariableKey = "enquiry_response";
    private const string EnquirerViewResponsePageLinkKey = "link_to_enquirer_view_all_responses_page";

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

    public async Task<bool> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var enquiryId = request.Data.EnquiryId;

        var tuitionPartnerId = request.Data.TuitionPartnerId;

        if (string.IsNullOrEmpty(request.Data.Token))
        {
            return false;
        }

        var magicLink =
            await _unitOfWork.MagicLinkRepository
                .SingleOrDefaultAsync(x => x.Token == request.Data.Token, null, true, cancellationToken);

        if (enquiryId == default || tuitionPartnerId == default) return false;

        var tpEnquiry = await _unitOfWork.TuitionPartnerEnquiryRepository
            .SingleOrDefaultAsync(x => x.EnquiryId == enquiryId &&
                                       x.TuitionPartnerId == tuitionPartnerId, "Enquiry,TuitionPartner,EnquiryResponse",
                true, cancellationToken);

        if (tpEnquiry == null) return false;

        request.Data.Email = tpEnquiry.Enquiry.Email;

        var notificationsRecipient = GetNotificationsRecipient(request, tpEnquiry.TuitionPartner.Name);

        var enquirerViewResponseMagicLink = new MagicLink()
        {
            Token = notificationsRecipient.Token!,
            EnquiryId = request.Data?.EnquiryId,
            MagicLinkTypeId = (int)MagicLinkType.EnquirerViewResponse
        };

        tpEnquiry.EnquiryResponse = new EnquiryResponse()
        {
            EnquiryResponseText = request.Data?.EnquiryResponseText!,
            EnquiryId = enquiryId,
            MagicLink = enquirerViewResponseMagicLink
        };

        tpEnquiry.MagicLinkId = magicLink?.Id;

        _unitOfWork.MagicLinkRepository.AddAsync(enquirerViewResponseMagicLink, cancellationToken);

        try
        {
            await _unitOfWork.Complete();

            await _notificationsClientService.SendEmailAsync(
                notificationsRecipient,
                EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred while trying to save the enquiry response. Error: {ex}", ex);
        }

        return false;
    }

    private NotificationsRecipientDto GetNotificationsRecipient(AddEnquiryResponseCommand request, string enquiryResponderText)
    {
        var generateRandomness
            = _aesEncryption.GenerateRandomToken();
        var token = _aesEncryption.Encrypt(
            $"EnquiryId={request.Data?.EnquiryId}&TuitionPartnerId={request.Data!.TuitionPartnerId}&Type={nameof(MagicLinkType.EnquirerViewResponse)}&{generateRandomness}");
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/respond/enquirer-response?token={token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Token = token,
            Personalisation = GetPersonalisation(request.Data?.EnquiryText!, request.Data?.EnquiryResponseText!, enquiryResponderText, pageLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string enquiryResponseText,
        string enquiryResponderText,
        string enquirerViewResponsePageLinkKey)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquiryResponderVariableKey, enquiryResponderText },
            { EnquiryResponseTextVariableKey, enquiryResponseText },
            { EnquirerViewResponsePageLinkKey, enquirerViewResponsePageLinkKey }
        };

        return personalisation;
    }
}