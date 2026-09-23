using ErrorOr;

namespace Sample.Application.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid externalId) =>
        Error.NotFound("PRODUCT_NOT_FOUND", $"Nenhum produto com o id {externalId}.");
}
