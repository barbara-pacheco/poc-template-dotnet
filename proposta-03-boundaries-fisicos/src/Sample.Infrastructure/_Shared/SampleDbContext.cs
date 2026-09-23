using Microsoft.EntityFrameworkCore;
using Sample.Domain.Products;

namespace Sample.Infrastructure._Shared;

/// <summary>
/// Porta de acesso ao Postgres via Entity Framework Core. Um DbSet por
/// entidade, usado pelos repositórios; o mapeamento de cada tabela vem do seu
/// IEntityTypeConfiguration (ex.: Products/ProductConfiguration.cs), aplicado
/// sozinho pelo ApplyConfigurationsFromAssembly.
/// </summary>
public class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleDbContext).Assembly);
}
