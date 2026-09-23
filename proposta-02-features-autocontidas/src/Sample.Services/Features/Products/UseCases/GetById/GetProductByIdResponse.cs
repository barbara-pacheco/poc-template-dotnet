using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Features.Products.UseCases.GetById;

public sealed record GetProductByIdResponse(Guid ExternalId, string Name, ProductStatus Status)
{
    public static GetProductByIdResponse From(Product product) =>
        new(product.ExternalId, product.Name, product.Status);
}
