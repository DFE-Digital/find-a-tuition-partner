using Application.Common.Interfaces;

namespace UI.Pages.Enquiry.Respond
{
    public class ResponsePageModel<T> : PageModel where T : PageModel
    {
        protected readonly ISessionService _sessionService;
        protected readonly IMediator _mediator;

        public ResponsePageModel(ISessionService sessionService, IMediator mediator)
        {
            _sessionService = sessionService;
            _mediator = mediator;

            if (_sessionService == null) throw new ArgumentNullException(nameof(sessionService));
            if (_mediator == null) throw new ArgumentNullException(nameof(mediator));
        }

        protected string GetSessionKey(string tuitionPartnerName, string enquiryRef)
        {
            return $"{tuitionPartnerName.ToSeoUrl()}_{enquiryRef}";
        }
    }
}
