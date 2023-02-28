using Application.Common.Models;
using KeyStageSubjectDictionary = System.Collections.Generic.Dictionary<Domain.Enums.KeyStage, Application.Common.Models.Selectable<string>[]>;

namespace UI.Pages;

public class WhichSubjects : PageModel
{
    private readonly IMediator _mediator;

    public WhichSubjects(IMediator mediator) => _mediator = mediator;

    public Command Data { get; set; } = new();

    public async Task OnGet(GetWhichSubjectQuery query)
    {
        Data = new Command(query)
        {
            AllSubjects = await _mediator.Send(query)
        };
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