namespace UI.Pages;

public class TriggerErrorModel : PageModel
{
    public void OnGet() => throw new InvalidOperationException("This exception is intentionally thrown");
}
