using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Domain.Products;
using Sample.Infrastructure._Shared;
using Sample.Infrastructure.Products;

namespace Sample.Infrastructure;

/// <summary>
/// Composition root da Infrastructure: registra só o que é responsabilidade
/// desta camada (o DbContext do Postgres e os repositórios de cada
/// componente). Chamado uma vez no Program.cs do host.
/// </summary>
public static class IoC
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);

        return services;
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<SampleDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IProductRepository, ProductRepository>();
    }
}
