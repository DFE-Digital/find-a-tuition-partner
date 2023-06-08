using Application.Commands.Admin;
using Application.Common.Models.Admin;

namespace UI.Pages.Admin
{
    public class ProcessEmails : PageModel
    {
        private readonly IMediator _mediator;

        public ProcessEmails(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty] public ProcessedEmailsModel Data { get; set; } = new();

        public async Task OnGetAsync()
        {
            var request = new ProcessEmailsCommand();
            Data = await _mediator.Send(request);
        }
    }
}
