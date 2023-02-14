using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<int>
{
    public EnquiryModel? Data { get; set; } = null!;
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
        var matchedSeoUrls = await _unitOfWork.TuitionPartnerRepository.GetMatchedSeoUrls(request.Data!.SelectedTuitionPartners!, cancellationToken);

        if (!matchedSeoUrls.Any()) return default;
        var tuitionPartnerEnquirySeoUrl = matchedSeoUrls.Select(selectedTuitionPartner =>
            new TuitionPartnerEnquirySeoUrl() { SeoUrl = selectedTuitionPartner }).ToList();

        var enquiry = new Enquiry()
        {
            EnquiryText = request.Data?.EnquiryText!,
            Email = request.Data?.Email!,
            TuitionPartnerEnquirySeoUrl = tuitionPartnerEnquirySeoUrl
        };

        _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

        var dataSaved = await _unitOfWork.Complete();

        return dataSaved ? enquiry.Id : default;
    }
}