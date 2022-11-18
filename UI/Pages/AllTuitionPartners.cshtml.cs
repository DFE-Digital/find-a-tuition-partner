using Application.Handlers;
using Domain.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Enums;
using UI.Models;

namespace UI.Pages;

public class AllTuitionPartners : PageModel
{
    private readonly IMediator _mediator;

    public AllTuitionPartners(IMediator mediator) => _mediator = mediator;

    [BindProperty(SupportsGet = true)]
    public SearchModel Data { get; set; } = new();

    public TuitionPartnerSearchResultsPage? Results { get; private set; }

    public async Task OnGet()
    {
        Data.From = ReferrerList.FullList;

        Results = await _mediator.Send(new SearchTuitionPartnerHandler.Command
        {
            Name = Data.Name,
            OrderBy = TuitionPartnerOrderBy.Name
        });
    }
}