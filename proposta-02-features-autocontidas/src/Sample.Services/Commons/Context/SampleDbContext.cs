using Microsoft.EntityFrameworkCore;
using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Commons.Context;

/// <summary>
/// Porta de acesso ao Postgres via Entity Framework Core. Um DbSet por
/// entidade, usado pelos repositórios; o mapeamento de cada tabela vem do seu
/// IEntityTypeConfiguration (ex.: Features/Products/Infrastructure/ProductConfiguration.cs), aplicado
/// sozinho pelo ApplyConfigurationsFromAssembly.
/// </summary>
public class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
}
