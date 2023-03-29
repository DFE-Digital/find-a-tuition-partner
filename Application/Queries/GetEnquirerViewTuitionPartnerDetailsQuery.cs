using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

public record GetEnquirerViewTuitionPartnerDetailsQuery(string SupportReferenceNumber, string MagicLinkToken) : IRequest<EnquirerViewTuitionPartnerDetailsModel?>;

public class GetEnquirerViewTuitionPartnerDetailsQueryHandler : IRequestHandler<GetEnquirerViewTuitionPartnerDetailsQuery, EnquirerViewTuitionPartnerDetailsModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetEnquirerViewTuitionPartnerDetailsQueryHandler> _logger;

    public GetEnquirerViewTuitionPartnerDetailsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetEnquirerViewTuitionPartnerDetailsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnquirerViewTuitionPartnerDetailsModel?> Handle(GetEnquirerViewTuitionPartnerDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewTuitionPartnerDetailsResponse(request.SupportReferenceNumber, request.MagicLinkToken);

        if (result == null)
        {
            _logger.LogWarning("Enquiry response not found for the given SupportReferenceNumber: {supportReferenceNumber} and MagicLinkToken: {magicLinkToken}",
                request.SupportReferenceNumber, request.MagicLinkToken);
        }
        return result;
    }
}