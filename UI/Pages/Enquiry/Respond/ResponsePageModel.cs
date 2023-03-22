using Application.Common.Interfaces;

namespace UI.Pages.Enquiry.Respond
{
    public class ResponsePageModel<T> : PageModel where T : PageModel
    {
        protected readonly ISessionService _sessionService;
        protected readonly IMediator _mediator;

        public ResponsePageModel(params object[] list)
        {
            foreach (var item in list)
            {
                if (item is ISessionService sessionService)
                {
                    _sessionService = sessionService;
                }
                if (item is IMediator mediator)
                {
                    _mediator = mediator;
                }
            }

            if (_sessionService == null) throw new ArgumentNullException("sessionService");
            if (_mediator == null) throw new ArgumentNullException("mediator");
        }

        protected string GetSessionKey(string tuitionPartnerName, string enquiryRef)
        {
            return $"{tuitionPartnerName.ToSeoUrl()}_{enquiryRef}";
        }
    }
}
