namespace Sample.Services.Features.Products.Domain;

/// <summary>
/// Situação do produto. Gravada como texto no banco (não como número), pra
/// que a coluna continue legível e reordenar o enum não corrompa dado antigo.
/// </summary>
public enum ProductStatus
{
    Active,
    Discontinued
}
