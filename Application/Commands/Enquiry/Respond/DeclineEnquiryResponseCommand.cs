using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Enquiry.Respond;

public record DeclineEnquiryResponseCommand(
    string SupportReferenceNumber,
    string TuitionPartnerSeoUrl) : IRequest<bool>
{ }

public class DeclineEnquiryResponseCommandHandler : IRequestHandler<DeclineEnquiryResponseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeclineEnquiryResponseCommandHandler> _logger;

    public DeclineEnquiryResponseCommandHandler(IUnitOfWork unitOfWork,
        ILogger<DeclineEnquiryResponseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(DeclineEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var result = new ResponseConfirmationModel();

        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            validationResult = $"The {nameof(DeclineEnquiryResponseCommandHandler)} {validationResult}";
            _logger.LogError(validationResult);
            throw new ArgumentException(validationResult);
        }

        var tpEnquiry = await _unitOfWork.TuitionPartnerEnquiryRepository
            .SingleOrDefaultAsync(x => x.Enquiry.SupportReferenceNumber == request.SupportReferenceNumber &&
                                       x.TuitionPartner.SeoUrl == request.TuitionPartnerSeoUrl, "Enquiry",
                true, cancellationToken);

        if (tpEnquiry == null)
        {
            var errorMessage = $"Unable to find TuitionPartnerEnquiry with Support Ref ('{request.SupportReferenceNumber}') and Tuition Partner SeoUrl ('{request.TuitionPartnerSeoUrl}')";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        if (tpEnquiry.TuitionPartnerDecinedEnquiry)
        {
            var errorMessage = $"Previously declined TuitionPartnerEnquiry with Support Ref ('{request.SupportReferenceNumber}') and Tuition Partner SeoUrl ('{request.TuitionPartnerSeoUrl}')";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        tpEnquiry.TuitionPartnerDecinedEnquiry = true;
        tpEnquiry.TuitionPartnerDecinedEnquiryDate = DateTime.UtcNow;

        await _unitOfWork.Complete();

        return true;
    }

    private static string? ValidateRequest(DeclineEnquiryResponseCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.TuitionPartnerSeoUrl))
        {
            return "TuitionPartnerSeoUrl is missing";
        }

        if (string.IsNullOrWhiteSpace(request.SupportReferenceNumber))
        {
            return "SupportReferenceNumber is missing";
        }

        return null;
    }
}