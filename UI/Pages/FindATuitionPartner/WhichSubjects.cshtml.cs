using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

public class WhichSubjects : PageModel
{
    private readonly IMediator mediator;

    public WhichSubjects(IMediator mediator) => this.mediator = mediator;

    [BindProperty]
    public Command Data { get; set; } = new();

    public async Task OnGet(Query query)
    {
        Data = await mediator.Send(query);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            Data = await mediator.Send(new Query(Data));
            return Page();
        }
        return RedirectToPage("Results", new SearchModel(Data));
    }

    public record Query : SearchModel, IRequest<Command>
    {
        public Query() { }
        public Query(SearchModel query) : base(query) { }
    }

    public record Command : SearchModel, IRequest<SearchModel>
    {
        public Dictionary<KeyStage, Selectable<string>[]> AllSubjects { get; set; } = new();
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

    public class Handler : IRequestHandler<Query, Command>
    {
        public Dictionary<KeyStage, string[]> KeyStageSubjects = new()
        {
            { KeyStage.KeyStage1, new[] { "Literacy", "Numeracy", "Science" } },
            { KeyStage.KeyStage2, new[] { "Literacy", "Numeracy", "Science" } },
            { KeyStage.KeyStage3, new[] { "Maths", "English", "Science", "Humanities", "Modern foreign languages" } },
            { KeyStage.KeyStage4, new[] { "Maths", "English", "Science", "Humanities", "Modern foreign languages" } },
        };

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            request.KeyStages ??= Array.Empty<KeyStage>();
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

            return new Command
            {
                Postcode = request.Postcode,
                AllSubjects = selectable,
            };
        }
    }
}