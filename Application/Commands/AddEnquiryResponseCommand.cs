using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;

namespace Application.Commands;

public record AddEnquiryResponseCommand : IRequest<bool>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var enquiryId = request.Data.EnquiryId;

        if (enquiryId == default) return false;
        _unitOfWork.EnquiryResponseRepository.AddAsync(new EnquiryResponse()
        {
            EnquiryResponseText = request.Data?.EnquiryResponseText!,
            EnquiryId = enquiryId
        }, cancellationToken);

        return await _unitOfWork.Complete();

    }
}