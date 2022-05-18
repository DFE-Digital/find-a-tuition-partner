using System.ComponentModel.DataAnnotations;
using Domain;

namespace UI.Models;

public class SubjectsSearchViewModel : SearchViewModelBase
{
    [Required(ErrorMessage = "Select the subject or subjects")]
    public ICollection<int>? SubjectIds { get; set; }
    public IEnumerable<Subject>? Subjects { get; set; }
}