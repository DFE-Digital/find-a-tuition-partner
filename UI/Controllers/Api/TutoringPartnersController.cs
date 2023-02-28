using Application.Common.Models;
using TuitionType = Domain.Enums.TuitionType;

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
    [ProducesResponseType(typeof(SearchResultsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Post([FromBody] GetSearchResultsQuery request)
    {
        request.TuitionType ??= TuitionType.Any;

        var result = await _sender.Send(request);

        return Ok(result);
    }
}