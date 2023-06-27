using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

namespace Application.Queries.Enquiry;

public record IsValidMagicLinkTokenQuery : IRequest<bool>
{
    public IsValidMagicLinkTokenQuery(string token, string supportReferenceNumber, string? tuitionPartnerSeoUrl = null, bool isTuitionPartnerResponse = false)
    {
        Token = token;
        SupportReferenceNumber = supportReferenceNumber;
        TuitionPartnerSeoUrl = tuitionPartnerSeoUrl;
        IsTuitionPartnerResponse = isTuitionPartnerResponse;
    }

    public string Token { get; set; }
    public string SupportReferenceNumber { get; set; }
    public string? TuitionPartnerSeoUrl { get; set; }
    public bool IsTuitionPartnerResponse { get; set; } = false;
    public bool ValidateEnquiryResponseStatus { get; set; } = true;
}

public class IsValidMagicLinkTokenQueryHandler : IRequestHandler<IsValidMagicLinkTokenQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IsValidMagicLinkTokenQueryHandler> _logger;

    public IsValidMagicLinkTokenQueryHandler(IUnitOfWork unitOfWork, ILogger<IsValidMagicLinkTokenQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(IsValidMagicLinkTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            _logger.LogInformation("request.Token is null or empty");
            return false;
        }
        if (string.IsNullOrWhiteSpace(request.SupportReferenceNumber))
        {
            _logger.LogInformation("request.SupportReferenceNumber is null or empty");
            return false;
        }
        if (request.IsTuitionPartnerResponse && string.IsNullOrWhiteSpace(request.TuitionPartnerSeoUrl))
        {
            _logger.LogInformation("request.TuitionPartnerSeoUrl is null or empty");
            return false;
        }

        var enquiry = await _unitOfWork.EnquiryRepository.GetEnquiryBySupportReferenceNumber(request.SupportReferenceNumber);

        if (enquiry == null)
        {
            _logger.LogInformation("No enquiry found for request.SupportReferenceNumber : {SupportReferenceNumber}", request.SupportReferenceNumber);
            return false;
        }

        if (request.IsTuitionPartnerResponse)
        {
            var tpEnquiry = enquiry.TuitionPartnerEnquiry
                .SingleOrDefault(x => x.MagicLink!.Token == request.Token && x.TuitionPartner.SeoUrl == request.TuitionPartnerSeoUrl);

            if (tpEnquiry == null)
            {
                _logger.LogInformation("Enquiry found but Tuition Partner token and Name not matched for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}; Token: {Token}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl, request.Token);
                return false;
            }

            if (tpEnquiry.EnquiryResponse != null)
            {
                _logger.LogInformation("Enquiry response already completed for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}; Token: {Token}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl, request.Token);
                return false;
            }

            if (tpEnquiry.ResponseCloseDate < DateTime.UtcNow)
            {
                _logger.LogInformation("Enquiry response expired for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}; Token: {Token}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl, request.Token);
                return false;
            }

            if (!tpEnquiry.TuitionPartner.IsActive)
            {
                _logger.LogInformation("Enquiry response Tuition Partner deactivated for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}; Token: {Token}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl, request.Token);
                return false;
            }
        }
        else
        {
            var enquirerMagicLinkToken = enquiry.MagicLink.Token;

            if (!enquirerMagicLinkToken.Equals(request.Token, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Enquiry found but enquirer token not matched for request.SupportReferenceNumber: {SupportReferenceNumber}; Supplied token: {Token}", request.SupportReferenceNumber, request.Token);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(request.TuitionPartnerSeoUrl))
            {
                var tpEnquiry = enquiry.TuitionPartnerEnquiry
                    .SingleOrDefault(x => x.TuitionPartner.SeoUrl == request.TuitionPartnerSeoUrl);

                if (tpEnquiry == null)
                {
                    _logger.LogInformation("Enquiry found for enquirer but TP Name not matched for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
                    return false;
                }

                if (tpEnquiry.EnquiryResponse == null)
                {
                    _logger.LogInformation("Enquiry found for enquirer but EnquiryResponse not matched for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
                    return false;
                }

                if (request.ValidateEnquiryResponseStatus && tpEnquiry.EnquiryResponse.EnquiryResponseStatusId == (int)EnquiryResponseStatus.NotInterested)
                {
                    _logger.LogInformation("Previously not interested enquiry matched for request.SupportReferenceNumber: {SupportReferenceNumber}; Tuition Partner URL: {TuitionPartnerSeoUrl}", request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
                    return false;
                }

            }
        }

        return true;
    }
}