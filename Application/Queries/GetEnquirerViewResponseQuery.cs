using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

public record GetEnquirerViewResponseQuery(string SupportReferenceNumber, string MagicLinkToken) : IRequest<EnquirerViewResponseModel?>;

public class GetEnquirerViewResponseQueryHandler : IRequestHandler<GetEnquirerViewResponseQuery, EnquirerViewResponseModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetEnquirerViewResponseQueryHandler> _logger;

    public GetEnquirerViewResponseQueryHandler(IUnitOfWork unitOfWork, ILogger<GetEnquirerViewResponseQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnquirerViewResponseModel?> Handle(GetEnquirerViewResponseQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewResponse(request.SupportReferenceNumber, request.MagicLinkToken);

        if (result == null)
        {
            _logger.LogWarning("Enquiry response not found for the given SupportReferenceNumber: {supportReferenceNumber} and MagicLinkToken: {magicLinkToken}",
                request.SupportReferenceNumber, request.MagicLinkToken);
        }
        return result;
    }
}