using Application.Common.Interfaces;
using Domain.Search;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

public record GetSearchLocationQuery(string Postcode) : IRequest<LocationFilterParameters?>;

public class GetSearchLocationQueryHandler : IRequestHandler<GetSearchLocationQuery, LocationFilterParameters?>
{
    private readonly ILogger<GetSearchLocationQueryHandler> _logger;

    private readonly ILocationFilterService _locationService;

    public GetSearchLocationQueryHandler(ILogger<GetSearchLocationQueryHandler> logger, ILocationFilterService locationService)
    {
        _logger = logger;
        _locationService = locationService;
    }

    public async Task<LocationFilterParameters?> Handle(GetSearchLocationQuery request, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetLocationFilterParametersAsync(request.Postcode);

        if (result == null)
        {
            _logger.LogInformation("Location response not found for the given postcode: {postcode}",
                request.Postcode);
        }
        return result;
    }
}