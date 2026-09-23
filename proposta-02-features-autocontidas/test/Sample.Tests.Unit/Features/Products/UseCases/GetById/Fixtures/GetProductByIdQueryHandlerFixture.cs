using Sample.Services.Features.Products.UseCases.GetById;
using Sample.Tests.Unit.Features.Products.UseCases.GetById.Mocks;

namespace Sample.Tests.Unit.Features.Products.UseCases.GetById.Fixtures;

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
