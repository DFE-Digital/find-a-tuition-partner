using Domain;

namespace UI.Models;

public class TuitionTypeSearchViewModel : SearchViewModelBase
{
    public int? TuitionTypeId { get; set; }
    public IEnumerable<TuitionType>? TuitionTypes { get; set; }
}