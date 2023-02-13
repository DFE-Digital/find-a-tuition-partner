using Application.Common.Models;

namespace Application.Queries;

public record GetTuitionPartnerQuery(
        string Id,
        bool ShowLocationsCovered = false,
        bool ShowFullPricing = false,
        bool ShowFullInfo = false)
    : IRequest<TuitionPartnerModel?>
{
    public SearchModel? SearchModel { get; set; }
}

