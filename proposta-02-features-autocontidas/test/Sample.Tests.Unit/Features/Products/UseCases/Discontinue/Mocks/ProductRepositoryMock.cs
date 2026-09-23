using Moq;
using Sample.Services.Features.Products.Domain;

namespace Sample.Tests.Unit.Features.Products.UseCases.Discontinue.Mocks;

public class ProductRepositoryMock : Mock<IProductRepository>
{
    public void ConfigureFindByExternalIdToReturn(Guid externalId, Product? product)
    {
        Setup(r => r.FindByExternalIdAsync(externalId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
    }

    public void VerifyUpdateWasCalledWith(Product product)
    {
        Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    public void VerifyUpdateWasNotCalled()
    {
        Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
