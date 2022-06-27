using Application;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UI.Pages.FindATuitionPartner;

public class Subjects : PageModel
{
    private readonly IMediator mediator;

    public Subjects(IMediator mediator) => this.mediator = mediator;

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
        public IEnumerable<Selectable> AllSubjects { get; set; } = new List<Selectable>();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.Subjects)
                .NotEmpty()
                .WithMessage("Select the subject or subjects");
        }
    }

    public class Handler : IRequestHandler<Query, Command>
    {
        private readonly INtpDbContext db;

        public Handler(INtpDbContext db) => this.db = db;

        public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
        {
            var q =
                from subject in await db.Subjects.ToListAsync(cancellationToken)
                join selected in request.Subjects on subject.Name equals selected into queryJoin
                from sub in queryJoin.DefaultIfEmpty()
                select new Selectable
                {
                    Name = subject.Name,
                    Selected = sub != null,
                };

            return new Command
            {
                Postcode = request.Postcode,
                AllSubjects = q.ToList(),
            };
        }
    }
}