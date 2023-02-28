using Application.Common.Interfaces;
using Application.Common.Models;
using UI.Pages.Enquiry.Build;
using KeyStageSubjectDictionary = System.Collections.Generic.Dictionary<Domain.Enums.KeyStage, Application.Common.Models.Selectable<string>[]>;

namespace UI.Pages;

public class WhichSubjects : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;

    public WhichSubjects(IMediator mediator, ISessionService sessionService)
    {
        _mediator = mediator;
        _sessionService = sessionService;
    }

    public Command Data { get; set; } = new();

    public async Task<IActionResult> OnGet(GetWhichSubjectQuery query)
    {
        Data = new Command(query)
        {
            AllSubjects = await _mediator.Send(query)
        };

        if (Data.From == ReferrerList.CheckYourAnswers)
        {
            var sessionId = Request.Cookies[StringConstants.SessionCookieName];

            if (sessionId == null) return RedirectToPage($"Enquiry/Build/{nameof(EnquirerEmail)}");

            var sessionValues = await _sessionService.RetrieveDataAsync(sessionId);

            if (sessionValues != null)
            {
                foreach (var sessionValue in sessionValues.Where(sessionValue => sessionValue.Key.Contains(StringConstants.Subjects)))
                {
                    query.Subjects = sessionValue.Value.Split(",", StringSplitOptions.RemoveEmptyEntries);
                }

                Data = new Command(query)
                {
                    AllSubjects = await _mediator.Send(query)
                };
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(Command data)
    {
        if (!ModelState.IsValid)
        {
            Data = data with
            {
                AllSubjects = await _mediator.Send(new GetWhichSubjectQuery(data))
            };
            return Page();
        }

        if (Request != null)
        {
            var sessionId = Request.Cookies[StringConstants.SessionCookieName];

            if (sessionId != null)
            {
                await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                {
                    { StringConstants.Subjects, string.Join(",", data.Subjects!)}
                });

                if (data.From == ReferrerList.CheckYourAnswers)
                {
                    return RedirectToPage($"Enquiry/Build/{nameof(CheckYourAnswers)}");
                }
            }
        }
        return RedirectToPage("SearchResults", new SearchModel(data));
    }

    public record Command : SearchModel, IRequest<SearchModel>
    {
        public Command() { }
        public Command(SearchModel query) : base(query) { }

        public KeyStageSubjectDictionary AllSubjects { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.Subjects)
                .NotEmpty()
                .WithMessage("Select the subject or subjects");

            RuleForEach(m => m.Subjects)
                .Must(x => KeyStageSubject.TryParse(x, out var _));
        }
    }
}