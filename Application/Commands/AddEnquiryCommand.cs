using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Build;
using Domain;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<int>
{
    public EnquiryBuildModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        //TODO - deal with no TPs selected - show a message on UI
        if (request.Data == null || request.Data.TuitionPartnersForEnquiry == null || request.Data.TuitionPartnersForEnquiry.Count == 0) return default;

        var tuitionPartnerEnquiry = request.Data.TuitionPartnersForEnquiry.Results.Select(selectedTuitionPartner =>
            new TuitionPartnerEnquiry() { TuitionPartnerId = selectedTuitionPartner.Id }).ToList();

        var enquiry = new Enquiry()
        {
            EnquiryText = request.Data?.EnquiryText!,
            Email = request.Data?.Email!,
            TuitionPartnerEnquiry = tuitionPartnerEnquiry
        };

        _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

        var dataSaved = await _unitOfWork.Complete();

        return dataSaved ? enquiry.Id : default;
    }
}