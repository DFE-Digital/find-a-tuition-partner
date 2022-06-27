using Application;
using Domain.Search;
using Infrastructure;
using Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace UI.Pages.Find
{
    public class Search : PageModel
    {
        private readonly IMediator mediator;

        public Search(IMediator mediator) => this.mediator = mediator;

        public async Task<IActionResult> OnGet()
        {
            var search = await mediator.Send(new Command());
            return RedirectToPage("Location", new { search.SearchId });
        }

        public record Command : IRequest<TuitionPartnerSearchRequestBuilder>
        {
            public Guid SearchId { get; } = Guid.NewGuid();
        }

        public class CommandHandler : IRequestHandler<Command, TuitionPartnerSearchRequestBuilder>
        {
            private readonly NtpDbContext context;

            public CommandHandler(NtpDbContext context)
            {
                this.context = context;
            }

            public async Task<TuitionPartnerSearchRequestBuilder> Handle(Command request, CancellationToken cancellationToken)
            {
                var state = new SearchState { SearchId = request.SearchId };

                var userSearch = new UserSearch
                {
                    Id = request.SearchId,
                    CreatedDate = DateTime.UtcNow,
                    SearchJson = JsonSerializer.Serialize(state)
                };

                context.UserSearches.Add(userSearch);

                await context.SaveChangesAsync();

                return new TuitionPartnerSearchRequestBuilder(state, null, null, null);
            }
        }
    }
}