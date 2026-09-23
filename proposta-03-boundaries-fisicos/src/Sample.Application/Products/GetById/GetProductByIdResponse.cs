using Sample.Domain.Products;

namespace Sample.Application.Products.GetById;

public sealed record GetProductByIdResponse(Guid ExternalId, string Name, ProductStatus Status)
{
    public static GetProductByIdResponse From(Product product) =>
        new(product.ExternalId, product.Name, product.Status);
}
