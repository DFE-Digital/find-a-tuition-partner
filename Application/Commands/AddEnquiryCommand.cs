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
        var matchedTps = await _unitOfWork.TuitionPartnerRepository
            .GetTuitionPartnersBySeoUrls(request.Data!.SelectedTuitionPartners!, cancellationToken);

        matchedTps = matchedTps.ToList();

        if (!matchedTps.Any()) return default;

        var tuitionPartnerEnquiry = matchedTps.Select(selectedTuitionPartner =>
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