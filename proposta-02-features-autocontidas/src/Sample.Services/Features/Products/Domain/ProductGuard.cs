namespace Sample.Services.Features.Products.Domain;

/// <summary>
/// Checagens das invariantes do Product. Chamado só de dentro da entidade
/// (por isso internal): quem segura um Product não tem como pular a regra.
/// </summary>
internal static class ProductGuard
{
    public static void NameIsNotEmpty(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProductException("O nome do produto não pode ser vazio.");
        }
    }

    public static void CanDiscontinue(Guid externalId, ProductStatus status)
    {
        if (status == ProductStatus.Discontinued)
        {
            throw new ProductAlreadyDiscontinuedException(externalId);
        }
    }
}
