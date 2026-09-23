using ErrorOr;
using Mediator;
using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Features.Products.UseCases.GetById;

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
