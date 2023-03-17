using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

public record GetEnquirerViewTutionPartnerDetailsQuery(int EnquiryId, int TuitionPartnerId) : IRequest<EnquirerViewTutionPartnerDetailsModel?>;

public class GetEnquirerViewTutionPartnerDetailsQueryHandler : IRequestHandler<GetEnquirerViewTutionPartnerDetailsQuery, EnquirerViewTutionPartnerDetailsModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetEnquirerViewTutionPartnerDetailsQueryHandler> _logger;

    public GetEnquirerViewTutionPartnerDetailsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetEnquirerViewTutionPartnerDetailsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnquirerViewTutionPartnerDetailsModel?> Handle(GetEnquirerViewTutionPartnerDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewTutionPartnerDetailsResponse(request.EnquiryId, request.TuitionPartnerId);

        if (result == null)
        {
            _logger.LogWarning("Enquiry response TP details not found for the given enquiryId: {enquiryId} and tuitionPartnerId: {tuitionPartnerId}",
                request.EnquiryId, request.TuitionPartnerId);
        }
        return result;
    }
}