using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sample.WebApi._Shared;

/// <summary>
/// Descobre via reflection toda classe que implementa IEndpoint e registra
/// sozinha (AddEndpoints, no boot) e mapeia a rota de cada uma
/// (MapEndpoints, depois do app montado) — ninguém precisa lembrar de
/// registrar um endpoint novo à mão.
/// </summary>
public static class EndpointExtensions
{
    public static void AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var descriptors = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IEndpoint).IsAssignableFrom(type))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(descriptors);
    }

    public static void MapEndpoints(this WebApplication app)
    {
        foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
            endpoint.Map(app);
    }
}
