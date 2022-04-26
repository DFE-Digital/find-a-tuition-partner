using System.ComponentModel.DataAnnotations;

namespace UI.Handlers.SearchTutoringPartners;

public class School
{
    public class Query
    {
        public string? Postcode { get; set; }
    }

    public class Command
    {
        [Required(ErrorMessage = "Enter a postcode")]
        public string? Postcode { get; set; }
    }
}