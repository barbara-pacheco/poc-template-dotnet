using Moq;
using Sample.Domain.Products;

namespace Sample.Tests.Unit.Application.Products.GetById.Mocks;

public class ProductRepositoryMock : Mock<IProductRepository>
{
    public void ConfigureFindByExternalIdToReturn(Guid externalId, Product? product)
    {
        Setup(r => r.FindByExternalIdAsync(externalId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
    }
}
