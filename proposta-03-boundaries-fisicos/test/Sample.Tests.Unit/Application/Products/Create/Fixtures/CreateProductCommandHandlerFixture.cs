using Sample.Application.Products.Create;
using Sample.Tests.Unit.Application.Products.Create.Mocks;

namespace Sample.Tests.Unit.Application.Products.Create.Fixtures;

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
