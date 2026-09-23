using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Sample.Application.Middleware;

namespace Sample.Application.Configuration;

/// <summary>
/// Composition root do host HTTP: Problem Details, o exception handler
/// global, JSON e Swagger. Chamado uma vez no Program.cs.
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
}
