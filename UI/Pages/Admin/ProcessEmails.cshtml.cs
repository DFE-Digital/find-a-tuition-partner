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
            var request = new ProcessEmailsCommand()
            {
                NotificationId = "708fec24-0a92-4089-8e6c-9b10f68a8526"
            };
            Data = await _mediator.Send(request);
        }
    }
}
