using UI.Pages;

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
    [ProducesResponseType(typeof(SearchResults.ResultsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Post([FromBody] SearchResults.Query request)
    {
        request.TuitionType ??= Enums.TuitionType.Any;

        var result = await _sender.Send(request);

        return Ok(result);
    }
}