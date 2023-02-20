using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Enums;
using Domain.Search;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Application.Commands;

public record SendEnquiryEmailCommand : IRequest<Unit>
{
    public EnquiryModel? Data { get; set; }
}

public class SendEnquiryEmailCommandHandler : IRequestHandler<SendEnquiryEmailCommand, Unit>
{
    private const string EnquiryTextVariableKey = "enquiry";
    private const string EnquiryResponseFormLinkKey = "link_to_tp_response_form";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IEncrypt _aesEncryption;

    public SendEnquiryEmailCommandHandler(IUnitOfWork unitOfWork,
        INotificationsClientService notificationsClientService, IEncrypt aesEncryption)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
        _aesEncryption = aesEncryption;
    }

    public async Task<Unit> Handle(SendEnquiryEmailCommand request, CancellationToken cancellationToken)
    {
        var matchedTps =
            await _unitOfWork.TuitionPartnerRepository.GetTuitionPartnersBySeoUrls(request.Data!.SelectedTuitionPartners!,
                cancellationToken);

        var notificationsRecipients = GetNotificationsRecipients(request,
            matchedTps);

        var hasEmailSent = await _notificationsClientService.SendEmailAsync(notificationsRecipients,
            EmailTemplateType.Enquiry);

        if (hasEmailSent)
        {
            var magicLinks = notificationsRecipients.Select(recipient => new MagicLink()
            { Token = recipient.Token, EnquiryId = request.Data?.EnquiryId, MagicLinkTypeId = (int)MagicLinkType.EnquiryRequest }).ToList();

            _unitOfWork.MagicLinkRepository.AddRangeAsync(magicLinks, cancellationToken);

            await _unitOfWork.Complete();
        }

        return Unit.Value;
    }

    private Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string responseFormLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquiryResponseFormLinkKey, responseFormLink }
        };

        return personalisation;
    }

    private List<NotificationsRecipientDto> GetNotificationsRecipients(SendEnquiryEmailCommand request,
        IEnumerable<TuitionPartnerResult> recipients)
    {
        return (from recipient in recipients
                let generateRandomness
                    = _aesEncryption.GenerateRandomToken()
                let token = _aesEncryption.Encrypt(
                    $"EnquiryId={request.Data?.EnquiryId}&Type={nameof(MagicLinkType.EnquiryRequest)}&{generateRandomness}")
                let formLink = $"{request.Data?.BaseServiceUrl}/enquiry-response?token={token}"
                select new NotificationsRecipientDto()
                {
                    Email = recipient.Email,
                    Token = token,
                    Personalisation = GetPersonalisation(request.Data?.EnquiryText!, formLink)
                }).ToList();
    }
}