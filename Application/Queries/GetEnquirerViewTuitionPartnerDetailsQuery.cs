using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

public record GetEnquirerViewTuitionPartnerDetailsQuery(int EnquiryId, int TuitionPartnerId) : IRequest<EnquirerViewTuitionPartnerDetailsModel?>;

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
            .GetEnquirerViewTuitionPartnerDetailsResponse(request.EnquiryId, request.TuitionPartnerId);

        if (result == null)
        {
            _logger.LogWarning("Enquiry response TP details not found for the given enquiryId: {enquiryId} and tuitionPartnerId: {tuitionPartnerId}",
                request.EnquiryId, request.TuitionPartnerId);
        }
        return result;
    }
}