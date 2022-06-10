using Application.Handlers;
using Domain;
using Domain.Search;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class TutoringPartnersController : ControllerBase
{
    private readonly ISender _sender;

    public TutoringPartnersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Post([FromBody] TuitionPartnerSearchRequest request)
    {
        var command = request.Adapt<SearchTuitionPartnerHandler.Command>();

        var result = await _sender.Send(command);

        return Ok(result);
    }
}