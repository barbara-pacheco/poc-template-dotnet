using Sample.Services.Features.Products.UseCases.Create;
using Sample.Tests.Unit.Features.Products.UseCases.Create.Mocks;

namespace Sample.Tests.Unit.Features.Products.UseCases.Create.Fixtures;

public class CreateProductCommandHandlerFixture
{
    protected ProductRepositoryMock ProductRepositoryMock { get; private set; }

    protected CreateProductCommandHandler Handler { get; private set; }

    protected CreateProductCommandHandlerFixture()
    {
        ProductRepositoryMock = new ProductRepositoryMock();

        Handler = new CreateProductCommandHandler(ProductRepositoryMock.Object);
    }
}
