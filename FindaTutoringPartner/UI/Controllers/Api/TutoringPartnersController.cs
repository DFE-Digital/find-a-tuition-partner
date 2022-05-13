using Domain;
using Domain.Search;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class TutoringPartnersController : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>), StatusCodes.Status200OK)]
    public void Post([FromBody] TuitionPartnerSearchRequest request)
    {
    }
}