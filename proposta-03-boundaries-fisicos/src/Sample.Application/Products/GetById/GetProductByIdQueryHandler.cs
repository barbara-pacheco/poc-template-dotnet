using ErrorOr;
using Mediator;
using Sample.Domain.Products;

namespace Sample.Application.Products.GetById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, ErrorOr<GetProductByIdResponse>>
{
    public async ValueTask<ErrorOr<GetProductByIdResponse>> Handle(
        GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByExternalIdAsync(query.ExternalId, cancellationToken);

        return product is null
            ? ProductErrors.NotFound(query.ExternalId)
            : GetProductByIdResponse.From(product);
    }
}
