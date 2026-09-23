using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Shared.Modules;

/// <summary>
/// Descobre via reflection toda classe que implementa IModule: no boot
/// (AddModules) registra as dependências de cada feature, e depois do app
/// montado (MapModules) mapeia as rotas — ninguém precisa lembrar de ligar
/// uma feature nova à mão.
/// </summary>
public static class ModuleExtensions
{
    public static IServiceCollection AddModules(this IServiceCollection services, Assembly assembly)
    {
        var modules = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IModule).IsAssignableFrom(type))
            .Select(type => (IModule)Activator.CreateInstance(type)!);

        foreach (var module in modules)
        {
            module.AddServices(services);
            services.AddSingleton(module);
        }

        return services;
    }

    public static WebApplication MapModules(this WebApplication app)
    {
        foreach (var module in app.Services.GetRequiredService<IEnumerable<IModule>>())
            module.MapEndpoints(app);

        return app;
    }
}
