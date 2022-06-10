using System.ComponentModel.DataAnnotations;
using Domain;

namespace UI.Models;

public class TutorTypesSearchViewModel : SearchViewModelBase
{
    [Required(ErrorMessage = "Select the tutor type or types")]
    public ICollection<int>? TutorTypeIds { get; set; }
    public IEnumerable<TutorType>? TutorTypes { get; set; }
}