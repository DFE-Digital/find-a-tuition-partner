using Application.Common.Interfaces;
using Application.Common.Models;

namespace UI.Pages.Enquiry.Build
{
    public class SubmittedConfirmation : PageModel
    {
        private readonly ISessionService _sessionService;

        public SubmittedConfirmation(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public SearchModel Data { get; set; } = new();

        public async Task OnGet(SearchModel data)
        {
            Data = data;

            var sessionId = Request.Cookies[StringConstants.SessionCookieName];

            if (sessionId != null)
            {
                await _sessionService.DeleteDataAsync();
            }
        }
    }
}
