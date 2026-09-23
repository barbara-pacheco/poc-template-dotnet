using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Services.Commons.Context;
using Sample.Shared.Behaviors;
using Sample.Shared.Modules;

namespace Sample.Services;

/// <summary>
/// Composition root do Services: registra o que é comum a todas as features
/// (Mediator com o pipeline de validação, validadores, DbContext) e liga o
/// Module.cs de cada feature. Chamado uma vez no Program.cs do host.
/// </summary>
public static class IoC
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddMediator(services);
        AddDbContext(services, configuration);

        services.AddModules(typeof(IoC).Assembly);

        return services;
    }

    private static void AddMediator(IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        services.AddValidatorsFromAssembly(typeof(IoC).Assembly);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<SampleDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());
    }
}
