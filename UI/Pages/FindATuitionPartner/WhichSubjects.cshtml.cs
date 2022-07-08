using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

using KeyStageSubjectDictionary = Dictionary<KeyStage, Selectable<string>[]>;

public class WhichSubjects : PageModel
{
    private readonly IMediator mediator;

    public WhichSubjects(IMediator mediator) => this.mediator = mediator;

    public KeyStageSubjectDictionary Subjects { get; set; } = new();

    public async Task OnGet(Query query)
    {
        Subjects = await mediator.Send(query);
    }

    [BindProperty(SupportsGet = true)]
    public SearchModel AllSearchParams { get; set; }

    public async Task<IActionResult> OnPost(Command data)
    {
        if (!ModelState.IsValid)
        {
            Subjects = await mediator.Send(new Query(data));
            return Page();
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
            { KeyStage.KeyStage1, new[] { "Literacy", "Numeracy", "Science" } },
            { KeyStage.KeyStage2, new[] { "Literacy", "Numeracy", "Science" } },
            { KeyStage.KeyStage3, new[] { "Maths", "English", "Science", "Humanities", "Modern foreign languages" } },
            { KeyStage.KeyStage4, new[] { "Maths", "English", "Science", "Humanities", "Modern foreign languages" } },
        };

        public async Task<KeyStageSubjectDictionary> Handle(Query request, CancellationToken cancellationToken)
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

            return selectable;
        }
    }
}