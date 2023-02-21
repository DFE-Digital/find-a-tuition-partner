using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Enums;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Application.Commands;

public record SendEnquirerViewAllResponsesEmailCommand : IRequest<Unit>
{
    public EnquiryModel? Data { get; set; }
}

public class SendEnquirerViewAllResponsesEmailCommandHandler : IRequestHandler<SendEnquirerViewAllResponsesEmailCommand, Unit>
{
    private const string EnquiryTextVariableKey = "enquiry";
    private const string EnquirerViewAllResponsesPageLinkKey = "link_to_enquirer_view_all_responses_page";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IEncrypt _aesEncryption;

    public SendEnquirerViewAllResponsesEmailCommandHandler(INotificationsClientService notificationsClientService,
        IUnitOfWork unitOfWork,
        IEncrypt aesEncryption)
    {
        _notificationsClientService = notificationsClientService;
        _unitOfWork = unitOfWork;
        _aesEncryption = aesEncryption;
    }

    public async Task<Unit> Handle(SendEnquirerViewAllResponsesEmailCommand request, CancellationToken cancellationToken)
    {
        var notificationsRecipient = GetNotificationsRecipients(request);

        var magicLink = new MagicLink()
        {
            Token = notificationsRecipient.Token,
            EnquiryId = request.Data?.EnquiryId,
            MagicLinkTypeId = (int)MagicLinkType.EnquirerViewAllResponses
        };

        _unitOfWork.MagicLinkRepository.AddAsync(magicLink, cancellationToken);

        await _unitOfWork.Complete();

        await _notificationsClientService.SendEmailAsync(
            new List<NotificationsRecipientDto>() { notificationsRecipient },
            EmailTemplateType.EnquirerViewResponses);

        return Unit.Value;
    }

    private NotificationsRecipientDto GetNotificationsRecipients(SendEnquirerViewAllResponsesEmailCommand request)
    {
        var generateRandomness
            = _aesEncryption.GenerateRandomToken();
        var token = _aesEncryption.Encrypt(
            $"EnquiryId={request.Data?.EnquiryId}&Type={nameof(MagicLinkType.EnquirerViewAllResponses)}&{generateRandomness}");
        var pageLink = $"{request.Data?.BaseServiceUrl}/enquirer-view-all-responses?token={token}";

        var result = new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            Token = token,
            Personalisation = GetPersonalisation(request.Data?.EnquiryText!, pageLink)
        };
        return result;
    }

    private Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string enquirerViewAllResponsesPageLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquirerViewAllResponsesPageLinkKey, enquirerViewAllResponsesPageLink }
        };

        return personalisation;
    }
}