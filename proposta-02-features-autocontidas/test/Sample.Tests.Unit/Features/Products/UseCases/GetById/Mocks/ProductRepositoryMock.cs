using Moq;
using Sample.Services.Features.Products.Domain;

namespace Sample.Tests.Unit.Features.Products.UseCases.GetById.Mocks;

public class ProductRepositoryMock : Mock<IProductRepository>
{
    public void ConfigureFindByExternalIdToReturn(Guid externalId, Product? product)
    {
        Setup(r => r.FindByExternalIdAsync(externalId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
    }
}
