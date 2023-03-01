using Application.Common.Interfaces;
using Application.Common.Models;
using UI.Pages.Enquiry.Build;

namespace UI.Pages
{
    public class WhichKeyStages : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ISessionService _sessionService;

        public WhichKeyStages(IMediator mediator, ISessionService sessionService)
        {
            _mediator = mediator;
            _sessionService = sessionService;
        }

        public Command Data { get; set; } = new();

        public async Task<IActionResult> OnGet(Query query)
        {
            Data = await _mediator.Send(query);

            if (query.From == ReferrerList.CheckYourAnswers)
            {
                var sessionId = Request.Cookies[StringConstants.SessionCookieName];

                if (sessionId == null) return RedirectToPage($"Enquiry/Build/{nameof(EnquirerEmail)}");

                var sessionValues = await _sessionService.RetrieveDataAsync(sessionId);

                if (sessionValues != null)
                {
                    foreach (var sessionValue in sessionValues.Where(sessionValue => sessionValue.Key.Contains(StringConstants.KeyStages)))
                    {
                        query.KeyStages = Enum.GetValues(typeof(KeyStage)).Cast<KeyStage>()
                            .Where(x => string.Join(" ", sessionValue).Contains(x.ToString())).ToArray();
                    }

                    Data = await _mediator.Send(query);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnGetSubmit(Command data)
        {
            if (ModelState.IsValid)
            {
                if (Request != null)
                {
                    var sessionId = Request.Cookies[StringConstants.SessionCookieName];

                    if (sessionId != null)
                    {
                        await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                        {
                            { StringConstants.KeyStages, string.Join(",", data.KeyStages!)}
                        });
                    }
                }

                Data = await _mediator.Send(new Query(data));

                return RedirectToPage("/WhichSubjects", new SearchModel(data));
            }
            else
            {
                Data = await _mediator.Send(new Query(data));
                return Page();
            }
        }

        public record Query : SearchModel, IRequest<Command>
        {
            public Query() { }
            public Query(SearchModel query) : base(query) { }
        }

        public record Command : SearchModel, IRequest<SearchModel>
        {
            public Command() { }
            public Command(SearchModel query) : base(query) { }
            public Selectable<KeyStage>[] AllKeyStages { get; set; } = Array.Empty<Selectable<KeyStage>>();
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.KeyStages)
                    .NotEmpty()
                    .WithMessage("Select at least one key stage");
            }
        }

        public class Handler : IRequestHandler<Query, Command>
        {
            private static readonly IEnumerable<KeyStage> AllKeyStages = new KeyStage[]
            {
                KeyStage.KeyStage1, KeyStage.KeyStage2, KeyStage.KeyStage3, KeyStage.KeyStage4
            };

            public Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                request.KeyStages ??= Array.Empty<KeyStage>();

                var selectable = AllKeyStages.Select(x => new Selectable<KeyStage>
                {
                    Name = x,
                    Selected = request.KeyStages.Contains(x)
                });

                return Task.FromResult(new Command(request)
                {
                    AllKeyStages = selectable.ToArray(),
                });
            }
        }
    }
}