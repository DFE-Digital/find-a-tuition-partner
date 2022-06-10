using Domain.Search;

namespace UI.Models;

public class TuitionPartnerViewModel : TuitionPartnerSearchResult
{
    public Guid SearchId { get; set; }
}