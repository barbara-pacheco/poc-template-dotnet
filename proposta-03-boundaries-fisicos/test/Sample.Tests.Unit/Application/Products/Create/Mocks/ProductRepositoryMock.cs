using Moq;
using Sample.Domain.Products;

namespace Sample.Tests.Unit.Application.Products.Create.Mocks;

public class ProductRepositoryMock : Mock<IProductRepository>
{
    public void VerifyAddWasCalledWith(string name)
    {
        Verify(r => r.AddAsync(
            It.Is<Product>(p => p.Name == name && p.Status == ProductStatus.Active),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
