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
        if (query.From == ReferrerList.CheckYourAnswers &&
            !await _sessionService.SessionDataExistsAsync()) return RedirectToPage("/Session/Timeout");

        Data = new Command(query)
        {
            AllSubjects = await _mediator.Send(query)
        };

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(Command data)
    {
        var redirectPage = nameof(WhichTuitionTypes);

        if (data.From == ReferrerList.CheckYourAnswers)
        {
            if (!await _sessionService.SessionDataExistsAsync())
                return RedirectToPage("/Session/Timeout");

            redirectPage = $"Enquiry/Build/{nameof(CheckYourAnswers)}";
        }

        if (!ModelState.IsValid)
        {
            Data = data with
            {
                AllSubjects = await _mediator.Send(new GetWhichSubjectQuery(data))
            };
            return Page();
        }

        return RedirectToPage(redirectPage, new SearchModel(data));
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