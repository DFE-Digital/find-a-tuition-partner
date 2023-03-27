using Application.Common.Interfaces;

namespace Application.Queries;

public record IsValidMagicLinkTokenQuery(string Token, string SupportReferenceNumber, bool IsValidateResponse = false) : IRequest<bool>;

public class IsValidMagicLinkTokenQueryHandler : IRequestHandler<IsValidMagicLinkTokenQuery, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public IsValidMagicLinkTokenQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(IsValidMagicLinkTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Token)) return false;

        var enquiry =
            await _unitOfWork.EnquiryRepository.GetEnquiryBySupportReferenceNumber(request.SupportReferenceNumber);

        if (enquiry == null)
        {
            return false;
        }

        if (request.IsValidateResponse)
        {
            var hasAnyEnquiryResponse = enquiry.TuitionPartnerEnquiry
                .Any(x => x.EnquiryResponse != null);

            var tpEnquiry = enquiry.TuitionPartnerEnquiry
                .SingleOrDefault(x => x.MagicLink!.Token == request.Token);

            if (hasAnyEnquiryResponse)
            {
                if (tpEnquiry?.EnquiryResponse != null) return false;
            }

            if (tpEnquiry?.ResponseCloseDate < DateTime.UtcNow)
                return false;
        }

        var hasEnquirerMagicLinkToken = enquiry.MagicLink != null;

        var enquirerMagicLinkToken = enquiry.MagicLink!.Token;

        if (hasEnquirerMagicLinkToken && !string.IsNullOrEmpty(enquirerMagicLinkToken)
                                      && enquirerMagicLinkToken.Equals(request.Token,
                                          StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var tpMagicLinkTokenQuery = enquiry.TuitionPartnerEnquiry
            .SingleOrDefault(x => x.MagicLink?.Token == request.Token);

        if (tpMagicLinkTokenQuery == null)
        {
            return false;
        }

        var hasTpMagicLinkToken = tpMagicLinkTokenQuery.MagicLink != null;

        var tpMagicLinkToken = tpMagicLinkTokenQuery.MagicLink!.Token;

        if (hasTpMagicLinkToken && !string.IsNullOrEmpty(tpMagicLinkToken) &&
            tpMagicLinkToken.Equals(request.Token, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}