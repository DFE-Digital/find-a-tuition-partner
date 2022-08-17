using Application;
using Domain.Search;
using Infrastructure.MetricLogging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using static Application.Handlers.SearchTuitionPartnerHandler;

namespace Infrastructure.Extensions;

public static class ServiceCollectionLoggingExtensions
{
    public static IServiceCollection LogKeyMetrics(this IServiceCollection services)
    {
        services.Decorate<ILocationFilterService, LoggingLocationFilterService>();
        services.AddScoped(
            typeof(IPipelineBehavior<Command, TuitionPartnerSearchResultsPage>),
            typeof(SearchTuitionPartnerHandlerLoggingBehaviour));
        return services;
    }
}