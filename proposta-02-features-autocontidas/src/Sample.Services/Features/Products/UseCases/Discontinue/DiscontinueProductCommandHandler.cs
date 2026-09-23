using ErrorOr;
using Mediator;
using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Features.Products.UseCases.Discontinue;

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
