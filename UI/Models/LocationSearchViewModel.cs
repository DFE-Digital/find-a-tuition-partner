using System.ComponentModel.DataAnnotations;

namespace UI.Models;

public class LocationSearchViewModel : SearchViewModelBase
{
    [Required(ErrorMessage = "Enter a postcode")]
    public string? Postcode { get; set; }
}