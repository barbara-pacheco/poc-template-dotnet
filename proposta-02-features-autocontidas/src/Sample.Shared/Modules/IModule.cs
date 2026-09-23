using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Shared.Modules;

/// <summary>
/// Contrato que o Module.cs de cada feature implementa: registra o que a
/// feature precisa no DI (repositórios etc.) e mapeia as rotas HTTP dela.
/// Implementar a interface JÁ é o registro — ver ModuleExtensions, que
/// descobre e liga todo mundo que implementa isso.
/// </summary>
public interface IModule
{
    void AddServices(IServiceCollection services);

    void MapEndpoints(IEndpointRouteBuilder app);
}
