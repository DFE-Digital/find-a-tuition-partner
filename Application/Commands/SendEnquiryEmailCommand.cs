using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.Commands;

public record SendEnquiryEmailCommand : IRequest<Unit>
{
    public EnquiryModel? Data { get; set; } = null!;
}

public class SendEnquiryEmailCommandHandler : IRequestHandler<SendEnquiryEmailCommand, Unit>
{
    private const string EnquiryTextVariableKey = "enquiry";

    private readonly INotificationsClientService _notificationsClientService;

    private readonly IUnitOfWork _unitOfWork;

    public SendEnquiryEmailCommandHandler(IUnitOfWork unitOfWork, INotificationsClientService notificationsClientService)
    {
        _unitOfWork = unitOfWork;
        _notificationsClientService = notificationsClientService;
    }
    public async Task<Unit> Handle(SendEnquiryEmailCommand request, CancellationToken cancellationToken)
    {
        var getMatchedSeoUrlsEmails = await _unitOfWork.TuitionPartnerRepository.
            GetMatchedSeoUrlsEmails(request.Data!.SelectedTuitionPartners!, cancellationToken);

        await _notificationsClientService.SendEmailAsync(getMatchedSeoUrlsEmails.ToList(),
            EmailTemplateType.Enquiry, GetPersonalisation(request));

        return Unit.Value;
    }

    private Dictionary<string, dynamic> GetPersonalisation(SendEnquiryEmailCommand request)
    {
        var personalisation = new Dictionary<string, dynamic>()
        {
            { EnquiryTextVariableKey, request.Data?.EnquiryText! }
        };

        return personalisation;
    }
}