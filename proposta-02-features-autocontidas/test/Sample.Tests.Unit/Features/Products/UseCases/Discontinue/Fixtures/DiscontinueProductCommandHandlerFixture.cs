using Sample.Services.Features.Products.UseCases.Discontinue;
using Sample.Tests.Unit.Features.Products.UseCases.Discontinue.Mocks;

namespace Sample.Tests.Unit.Features.Products.UseCases.Discontinue.Fixtures;

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
