using Application.Behaviours;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddMediatR(typeof(AssemblyReference));

        AssemblyScanner.FindValidatorsInAssembly(typeof(AssemblyReference).Assembly)
            .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}