using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<bool>
{
    public EnquiryModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        var enquiry = new Enquiry()
        {
            EnquiryText = request.Data?.EnquiryText!,
            Email = request.Data?.Email!,
            TuitionPartners = request.Data?.SelectedTuitionPartners!
        };

        _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

        return await _unitOfWork.Complete();
    }
}