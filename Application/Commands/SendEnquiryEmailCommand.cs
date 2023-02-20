using Application.Common.DTO;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Domain.Enums;

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
        var enquirerEmailForTestingPurposes = request.Data?.Email!;

        var getMatchedSeoUrlsEmails =
            await _unitOfWork.TuitionPartnerRepository.GetMatchedSeoUrlsEmails(request.Data!.SelectedTuitionPartners!,
                cancellationToken);

        var tpNotificationsRecipients = GetTuitionPartnerNotificationsRecipients(request,
            getMatchedSeoUrlsEmails.ToList(), enquirerEmailForTestingPurposes);

        //Save to the database, since the magic links need to exist before sending the emails
        var magicLinks = tpNotificationsRecipients.Select(recipient => new MagicLink()
        { Token = recipient.Token!, EnquiryId = request.Data?.EnquiryId }).ToList();

        _unitOfWork.MagicLinkRepository.AddRangeAsync(magicLinks, cancellationToken);

        await _unitOfWork.Complete();

        //Send the enquirer email
        await _notificationsClientService.SendEmailAsync(
            GetEnquirerNotificationsRecipient(request, enquirerEmailForTestingPurposes),
            EmailTemplateType.EnquirySubmittedConfirmationToEnquirer);

        //Send the TP emails
        await _notificationsClientService.SendEmailAsync(tpNotificationsRecipients,
            EmailTemplateType.EnquirySubmittedToTp);

        return Unit.Value;
    }

    private static Dictionary<string, dynamic> GetPersonalisation(string enquiryText, string responseFormLink)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, enquiryText },
            { EnquiryResponseFormLinkKey, responseFormLink }
        };

        return personalisation;
    }

    private List<NotificationsRecipientDto> GetTuitionPartnerNotificationsRecipients(SendEnquiryEmailCommand request,
        List<string> recipients, string enquirerEmailForTestingPurposes)
    {
        return (from recipient in recipients
                let generateRandomness
                    = _aesEncryption.GenerateRandomToken()
                let token = _aesEncryption.Encrypt(
                    $"EnquiryId={request.Data?.EnquiryId}&{generateRandomness}")
                let formLink = $"{request.Data?.BaseServiceUrl}/enquiry-response?token={token}"
                select new NotificationsRecipientDto()
                {
                    Email = recipient,
                    EnquirerEmailForTestingPurposes = enquirerEmailForTestingPurposes,
                    Token = token,
                    Personalisation = GetPersonalisation(request.Data?.EnquiryText!, formLink)
                }).ToList();
    }

    private static NotificationsRecipientDto GetEnquirerNotificationsRecipient(SendEnquiryEmailCommand request,
        string enquirerEmailForTestingPurposes)
    {
        return new NotificationsRecipientDto()
        {
            Email = request.Data?.Email!,
            EnquirerEmailForTestingPurposes = enquirerEmailForTestingPurposes,
            Personalisation = new Dictionary<string, dynamic>()
            {
                { EnquiryTextVariableKey, request.Data?.EnquiryText! }
            }
        };
    }
}