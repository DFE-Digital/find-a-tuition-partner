using Application;
using Infrastructure.MetricLogging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionLoggingExtensions
{
    public static IServiceCollection LogKeyMetrics(this IServiceCollection services)
    {
        services.Decorate<ILocationFilterService, LoggingLocationFilterService>();
        services.Decorate<ITuitionPartnerService, LoggingTuitionPartnerService>();
        return services;
    }
}