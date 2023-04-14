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
                //Errored email = "763404e0-47ed-4b72-93a7-843d31597206"
                //OK Email = "708fec24-0a92-4089-8e6c-9b10f68a8526"
                NotificationId = "763404e0-47ed-4b72-93a7-843d31597206"
            };
            Data = await _mediator.Send(request);
        }
    }
}
