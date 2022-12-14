namespace UI.Pages
{
    public class ContactUsModel : PageModel
    {
        public string? ReturnPath { get; set; }

        public IActionResult OnGet()
        {
            ReturnPath = Request.Headers["Referer"].ToString();

            return Page();
        }

        public IActionResult OnPost(string returnUrl)
        {
            ReturnPath = Request.Headers["Referer"].ToString();

            return Page();
        }
    }
}
