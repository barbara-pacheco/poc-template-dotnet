using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Sample.Application._Shared;

namespace Sample.Application;

/// <summary>
/// Composition root do Application: registra o que é responsabilidade desta
/// camada — o Mediator (com o pipeline de validação) e os validadores do
/// FluentValidation. Chamado uma vez no Program.cs do host.
/// </summary>
public static class IoC
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        services.AddValidatorsFromAssembly(typeof(IoC).Assembly);

        return services;
    }
}
