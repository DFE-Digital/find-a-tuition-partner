using System.ComponentModel.DataAnnotations;

namespace UI.Models;

public abstract class SearchViewModelBase
{
    [Required(ErrorMessage = "Invalid search identifier. Please restart your search")]
    public Guid SearchId { get; set; }
}