using Application.Common.Models;

namespace UI.Pages
{
    public class CompareListClearConfirm : PageModel
    {
        private readonly ILogger<TuitionPartner> _logger;
        private readonly IMediator _mediator;

        public CompareListClearConfirm(ILogger<TuitionPartner> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Query Data { get; set; } = new();

        public void OnGet(Query data)
        {
            Data = data;
        }

        public async Task<IActionResult> OnPostRemoveAsync(Query data)
        {
            if (!ModelState.IsValid) return Page();

            //Remove all TPs
            await _mediator.Send(new RemoveAllCompareListedTuitionPartnersCommand());

            _logger.LogInformation("Cleared full compare list");

            return RedirectToPage("compare-list", data);
        }

        public record Query : SearchModel
        {
        };
    }
}
