using Moq;
using Sample.Services.Features.Products.Domain;

namespace Sample.Tests.Unit.Features.Products.UseCases.Create.Mocks;

public class ProductRepositoryMock : Mock<IProductRepository>
{
    public void VerifyAddWasCalledWith(string name)
    {
        Verify(r => r.AddAsync(
            It.Is<Product>(p => p.Name == name && p.Status == ProductStatus.Active),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
