namespace Sample.Services.Features.Products.Domain;

/// <summary>
/// Porta de persistência do Product, implementada na Infrastructure. O
/// repositório persiste e traduz, nada mais: não decide regra nem devolve
/// erro de negócio.
/// </summary>
public interface IProductRepository
{
    Task<Product?> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task UpdateAsync(Product product, CancellationToken cancellationToken);
}
