namespace UI.Pages
{
    public class ShortlistClearConfirm : PageModel
    {
        private readonly IMediator _mediator;

        public ShortlistClearConfirm(IMediator mediator) => _mediator = mediator;

        public Query Data { get; set; } = new();

        public void OnGet(Query data)
        {
            Data = data;
        }

        public async Task<IActionResult> OnPostRemoveAsync(Query data)
        {
            if (!ModelState.IsValid) return Page();

            //Remove all TPs
            await _mediator.Send(new RemoveAllTuitionPartnersCommand());

            return RedirectToPage("shortlist", data.ToRouteData());
        }

        public record Query : SearchModel
        {
        };
    }
}
