using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.Search;

namespace UI.Models;

public class TuitionPartnerSearchResultsViewModel
{
    [Required(ErrorMessage = "Invalid search identifier. Please restart your search")]
    public Guid SearchId { get; set; }
    public LocationFilterParameters LocationFilterParameters { get; set; } = null!;
    [Required(ErrorMessage = "Select the subject or subjects")]
    public ICollection<int> SubjectIds { get; set; } = null!;
    public IEnumerable<Subject> Subjects { get; set; } = null!;
    [Required(ErrorMessage = "Select the tuition type or types")]
    public ICollection<int> TuitionTypeIds { get; set; } = null!;
    public IEnumerable<TuitionType> TuitionTypes { get; set; } = null!;
    public SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner> SearchResultsPage { get; set; } = null!;
}