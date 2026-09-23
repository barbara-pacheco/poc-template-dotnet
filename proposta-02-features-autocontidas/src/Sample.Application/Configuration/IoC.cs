using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Sample.Application.Middleware;
using Sample.Services.Commons.Context;
using Sample.Shared.Behaviors;
using Sample.Shared.Modules;

namespace Sample.Application.Configuration;

/// <summary>
/// Composition root: tudo que é configuração de DI fica aqui. AddApplication
/// registra o host HTTP (Problem Details, exception handler global, JSON,
/// Swagger); AddServices registra o que é comum a todas as features (Mediator
/// com o pipeline de validação, validadores, DbContext) e liga o Module.cs de
/// cada feature. Chamados uma vez no Program.cs.
/// </summary>
public static class IoC
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        AddJson(services);
        AddSwagger(services);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddMediator(services);
        AddDbContext(services, configuration);

        services.AddModules(typeof(SampleDbContext).Assembly);

        return services;
    }

    private static void AddJson(IServiceCollection services)
    {
        // Enum sai como texto no JSON ("Active"), não como número: o número é
        // detalhe de implementação e mudaria se alguém reordenasse o enum.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // O Swashbuckle gera o schema a partir das opções de JSON do MVC, não
        // das de Minimal API acima. Sem esta linha o Swagger documentaria o enum
        // como número enquanto a API responde texto.
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", document, null), [] }
            });
        });
    }

    private static void AddMediator(IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        services.AddValidatorsFromAssembly(typeof(SampleDbContext).Assembly);
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
