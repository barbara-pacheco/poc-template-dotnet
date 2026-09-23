using ErrorOr;
using Mediator;
using Sample.Services.Features.Products.Domain;

namespace Sample.Services.Features.Products.UseCases.Create;

public sealed class CreateProductCommandHandler(IProductRepository productRepository)
    : ICommandHandler<CreateProductCommand, ErrorOr<CreateProductResponse>>
{
    public async ValueTask<ErrorOr<CreateProductResponse>> Handle(
        CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = Product.Create(command.Name.Trim());

        await productRepository.AddAsync(product, cancellationToken);

        return CreateProductResponse.From(product);
    }
}
