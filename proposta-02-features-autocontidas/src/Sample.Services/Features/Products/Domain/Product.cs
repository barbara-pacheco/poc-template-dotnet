namespace Sample.Services.Features.Products.Domain;

/// <summary>
/// Entidade de domínio do componente de exemplo.
///
/// A regra vive aqui, não no handler: o que vale pra um Product vale pra
/// todo caso de uso que segurar um, inclusive os que ainda não existem.
/// </summary>
public sealed class Product
{
    public Guid ExternalId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ProductStatus Status { get; private set; }

    private Product()
    {
    }

    public static Product Create(string name)
    {
        ProductGuard.NameIsNotEmpty(name);

        return new Product
        {
            ExternalId = Guid.CreateVersion7(),
            Name = name,
            Status = ProductStatus.Active
        };
    }

    public void Discontinue()
    {
        ProductGuard.CanDiscontinue(ExternalId, Status);

        Status = ProductStatus.Discontinued;
    }
}
