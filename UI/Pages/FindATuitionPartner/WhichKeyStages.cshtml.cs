using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.FindATuitionPartner
{
    public class WhichKeyStages : PageModel
    {
        private readonly IMediator mediator;

        public WhichKeyStages(IMediator mediator) => this.mediator = mediator;

        [BindProperty]
        public Command Data { get; set; } = new();

        public record Query : SearchModel, IRequest<Command>
        {
            public Query() { }
            public Query(SearchModel query) : base(query) { }
        }

        public async Task OnGet(Query query)
        {
            Data = await mediator.Send(query);
        }

        public record Command : SearchModel, IRequest<SearchModel>
        {
            public Command() { }
            public Command(SearchModel query) : base(query) { }
            public Selectable<KeyStage>[] AllKeyStages { get; set; } = Array.Empty<Selectable<KeyStage>>();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Data = await mediator.Send(new Query(Data));
                return Page();
            }
            return RedirectToPage(nameof(WhichSubjects), new SearchModel(Data));
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.KeyStages)
                    .NotEmpty()
                    .WithMessage("Select the key stage or key stages");
            }
        }

        public class Handler : IRequestHandler<Query, Command>
        {
            static readonly IEnumerable<KeyStage> AllKeyStages = new KeyStage[]
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
