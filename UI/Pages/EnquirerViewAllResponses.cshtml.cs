using Application.Common.Models;

namespace UI.Pages;

public class EnquirerViewAllResponses : PageModel
{
    [BindProperty] public List<EnquirerViewAllResponsesModel> Data { get; set; } = new();
    [ViewData] public string? ErrorMessage { get; set; }

    public void OnGet()
    {

    }
}