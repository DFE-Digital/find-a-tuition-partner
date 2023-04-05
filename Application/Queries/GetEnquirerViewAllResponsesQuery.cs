using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace Application.Queries;

public record GetEnquirerViewAllResponsesQuery(string SupportReferenceNumber) : IRequest<EnquirerViewAllResponsesModel>;

public class GetEnquirerViewAllResponsesQueryHandler : IRequestHandler<GetEnquirerViewAllResponsesQuery, EnquirerViewAllResponsesModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnquirerViewAllResponsesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnquirerViewAllResponsesModel> Handle(GetEnquirerViewAllResponsesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EnquiryRepository
            .GetEnquirerViewAllResponses(request.SupportReferenceNumber);
    }
}