using Sample.Application.Products.Discontinue;
using Sample.Tests.Unit.Application.Products.Discontinue.Mocks;

namespace Sample.Tests.Unit.Application.Products.Discontinue.Fixtures;

public class DiscontinueProductCommandHandlerFixture
{
    protected ProductRepositoryMock ProductRepositoryMock { get; private set; }

    protected DiscontinueProductCommandHandler Handler { get; private set; }

    protected DiscontinueProductCommandHandlerFixture()
    {
        ProductRepositoryMock = new ProductRepositoryMock();

        Handler = new DiscontinueProductCommandHandler(ProductRepositoryMock.Object);
    }
}
