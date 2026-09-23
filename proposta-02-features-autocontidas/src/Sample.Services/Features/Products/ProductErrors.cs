using ErrorOr;

namespace Sample.Services.Features.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid externalId) =>
        Error.NotFound("PRODUCT_NOT_FOUND", $"Nenhum produto com o id {externalId}.");
}
