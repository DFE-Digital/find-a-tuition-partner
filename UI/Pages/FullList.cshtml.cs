using Application.Handlers;
using Domain.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages;

public class FullList : PageModel
{
    private readonly IMediator _mediator;

    public FullList(IMediator mediator) => _mediator = mediator;

    public TuitionPartnerSearchResultsPage? Results { get; private set; }

    public async Task OnGet()
    {
        Results = await _mediator.Send(new SearchTuitionPartnerHandler.Command
        {
            OrderBy = TuitionPartnerOrderBy.Name
        });
    }
}