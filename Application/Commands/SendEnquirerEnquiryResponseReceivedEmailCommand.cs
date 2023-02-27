using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public record SendEnquirerEnquiryResponseReceivedEmailCommand : IRequest<Unit>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class SendEnquirerEnquiryResponseReceivedEmailCommandHandler : IRequestHandler<SendEnquirerEnquiryResponseReceivedEmailCommand, Unit>
{
    private const string EnquiryTextVariableKey = "enquiry";
    private const string EnquiryResponderVariableKey = "enquiry_responder";
    private const string EnquiryResponseTextVariableKey = "enquiry_response";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<SendEnquirerEnquiryResponseReceivedEmailCommandHandler> _logger;

    public SendEnquirerEnquiryResponseReceivedEmailCommandHandler(INotificationsClientService notificationsClientService,
        IUnitOfWork unitOfWork, ILogger<SendEnquirerEnquiryResponseReceivedEmailCommandHandler> logger)
    {
        _notificationsClientService = notificationsClientService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(SendEnquirerEnquiryResponseReceivedEmailCommand request,
        CancellationToken cancellationToken)
    {
        var enquirerEnquiryResponseReceivedData =
            await _unitOfWork.MagicLinkRepository.
                GetEnquirerEnquiryResponseReceivedData(request.Data.EnquiryId!, request.Data.TuitionPartnerId);

        if (enquirerEnquiryResponseReceivedData == null)
        {
            _logger.LogError("Unable to send enquirer enquiry response received email. " +
                             "Can't find magic link token with the type {type} by enquiry Id {enquiryId}",
                MagicLinkType.EnquirerViewAllResponses.ToString(), request.Data.EnquiryId);

            return Unit.Value;
        }

        request.Data.Token = enquirerEnquiryResponseReceivedData.Token!;
        request.Data.Email = enquirerEnquiryResponseReceivedData.Email!;
        request.Data.EnquiryText = enquirerEnquiryResponseReceivedData.EnquiryText!;

        var notificationsRecipient = GetNotificationsRecipient(request, enquirerEnquiryResponseReceivedData.TuitionPartnerName);

        await _notificationsClientService.SendEmailAsync(
            notificationsRecipient,
            EmailTemplateType.EnquiryResponseReceivedConfirmationToEnquirer);
        return Unit.Value;
    }

    private NotificationsRecipientDto GetNotificationsRecipient(SendEnquirerEnquiryResponseReceivedEmailCommand request, string enquiryResponderText)
    {
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquiry/respond/all-enquirer-responses?token={request.Data?.Token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = request.Data?.Email!,
            Personalisation = GetPersonalisation(request.Data?.EnquiryText!,
                request.Data?.EnquiryResponseText!, enquiryResponderText, pageLink)
        };
        return result;
    }

    private static Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string enquiryResponseText,
        string enquiryResponderText,
        string enquirerViewAllResponsesPageLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquiryResponderVariableKey, enquiryResponderText },
            { EnquiryResponseTextVariableKey, enquiryResponseText },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLink }
        };

        return personalisation;
    }
}