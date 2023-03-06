using Application.Common.Interfaces;

namespace UI.Pages.Session
{
    public class Clear : PageModel
    {
        private readonly ISessionService _sessionService;

        public Clear(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task OnGetAsync()
        {
            await _sessionService.ClearAsync();
        }
    }
}
