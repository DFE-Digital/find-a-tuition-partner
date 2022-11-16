using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages
{
    public class WhichKeyStages : PageModel
    {
        private readonly IMediator _mediator;

        public WhichKeyStages(IMediator mediator) => _mediator = mediator;

        public Command Data { get; set; } = new();

        public async Task OnGet(Query query) => Data = await _mediator.Send(query);

        public async Task<IActionResult> OnGetSubmit(Command data)
        {
            if (ModelState.IsValid)
            {
                return RedirectToPage(nameof(WhichSubjects), new SearchModel(data));
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