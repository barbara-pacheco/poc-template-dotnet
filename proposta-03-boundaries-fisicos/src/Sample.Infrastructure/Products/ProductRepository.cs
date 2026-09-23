using Microsoft.EntityFrameworkCore;
using Sample.Domain.Products;
using Sample.Infrastructure._Shared;

namespace Sample.Infrastructure.Products;

/// <summary>
/// Implementação EF Core da porta IProductRepository. Persiste e nada mais:
/// não carimba campo, não decide regra, não devolve erro de negócio.
/// </summary>
internal sealed class ProductRepository(SampleDbContext dbContext) : IProductRepository
{
    // Sem AsNoTracking de propósito: a chave "Id" é shadow property e só
    // sobrevive enquanto o EF Core rastreia a entidade — sem ela, o
    // UpdateAsync não saberia qual linha atualizar.
    public Task<Product?> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken) =>
        dbContext.Products
            .FirstOrDefaultAsync(product => product.ExternalId == externalId, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
