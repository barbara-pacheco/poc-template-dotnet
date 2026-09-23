using Sample.Application.Products.GetById;
using Sample.Tests.Unit.Application.Products.GetById.Mocks;

namespace Sample.Tests.Unit.Application.Products.GetById.Fixtures;

public class GetProductByIdQueryHandlerFixture
{
    protected ProductRepositoryMock ProductRepositoryMock { get; private set; }

    protected GetProductByIdQueryHandler Handler { get; private set; }

    protected GetProductByIdQueryHandlerFixture()
    {
        ProductRepositoryMock = new ProductRepositoryMock();

        Handler = new GetProductByIdQueryHandler(ProductRepositoryMock.Object);
    }
}
