using ErrorOr;
using Mediator;
using Sample.Domain.Products;

namespace Sample.Application.Products.Discontinue;

public sealed class DiscontinueProductCommandHandler(IProductRepository productRepository)
    : ICommandHandler<DiscontinueProductCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(
        DiscontinueProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByExternalIdAsync(command.ExternalId, cancellationToken);

        if (product is null)
        {
            return ProductErrors.NotFound(command.ExternalId);
        }

        product.Discontinue();

        await productRepository.UpdateAsync(product, cancellationToken);

        return Result.Success;
    }
}
