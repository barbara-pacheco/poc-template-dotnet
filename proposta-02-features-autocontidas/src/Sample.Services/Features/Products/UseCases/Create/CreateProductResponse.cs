using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Features.Products.UseCases.Create;

public sealed record CreateProductResponse(Guid ExternalId, string Name, ProductStatus Status)
{
    public static CreateProductResponse From(Product product)
    {
        return new CreateProductResponse(
            product.ExternalId, 
            product.Name, 
            product.Status);
    }
}
