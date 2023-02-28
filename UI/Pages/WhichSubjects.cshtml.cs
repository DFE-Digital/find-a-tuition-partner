using Application.Common.Interfaces;
using Application.Common.Models;
using UI.Pages.Enquiry.Build;

namespace UI.Pages;

using KeyStageSubjectDictionary = Dictionary<KeyStage, Selectable<string>[]>;

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

    public async Task<IActionResult> OnGet(Query query)
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
                AllSubjects = await _mediator.Send(new Query(data))
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

    public record Query : SearchModel, IRequest<KeyStageSubjectDictionary>
    {
        public Query() { }
        public Query(SearchModel query) : base(query) { }
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

    public class Handler : IRequestHandler<Query, KeyStageSubjectDictionary>
    {
        public Dictionary<KeyStage, string[]> KeyStageSubjects = new()
        {
            { KeyStage.KeyStage1, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName() } },
            { KeyStage.KeyStage2, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName() } },
            { KeyStage.KeyStage3, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName(), Subject.Humanities.DisplayName(), Subject.ModernForeignLanguages.DisplayName() } },
            { KeyStage.KeyStage4, new[] { Subject.English.DisplayName(), Subject.Maths.DisplayName(), Subject.Science.DisplayName(), Subject.Humanities.DisplayName(), Subject.ModernForeignLanguages.DisplayName() } },
        };

        public Task<KeyStageSubjectDictionary> Handle(Query request, CancellationToken cancellationToken)
        {
            request.KeyStages ??= new[]
            {
                KeyStage.KeyStage1,
                KeyStage.KeyStage2,
                KeyStage.KeyStage3,
                KeyStage.KeyStage4,
            };
            request.Subjects ??= Array.Empty<string>();

            var selectable = KeyStageSubjects
                .Where(x => request.KeyStages.Contains(x.Key))
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Select(subject => new Selectable<string>
                    {
                        Name = subject,
                        Selected = request.Subjects.ParseKeyStageSubjects().Any(s => s.KeyStage == x.Key && s.Subject == subject),
                    }).ToArray()
                );

            return Task.FromResult(selectable);
        }
    }
}