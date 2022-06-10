using System.ComponentModel.DataAnnotations;
using Domain;

namespace UI.Models;

public class TuitionTypeSearchViewModel : SearchViewModelBase
{
    [Required(ErrorMessage = "Select the tuition type or types")]
    public ICollection<int>? TuitionTypeIds { get; set; }
    public IEnumerable<TuitionType>? TuitionTypes { get; set; }
}