using AwesomeAssertions;
using ErrorOr;
using Sample.Application.Products.Discontinue;
using Sample.Domain.Products;
using Sample.Tests.Unit.Application.Products.Discontinue.Fakers;
using Sample.Tests.Unit.Application.Products.Discontinue.Fixtures;

namespace Sample.Tests.Unit.Application.Products.Discontinue;

public class DiscontinueProductCommandHandlerTests : DiscontinueProductCommandHandlerFixture
{
    [Fact]
    public async Task Handle_WithActiveProduct_DiscontinuesAndPersists()
    {
        // Arrange
        var product = ProductFaker.Valid();
        ProductRepositoryMock.ConfigureFindByExternalIdToReturn(product.ExternalId, product);

        // Act
        var result = await Handler.Handle(new DiscontinueProductCommand(product.ExternalId), default);

        // Assert
        result.IsError.Should().BeFalse();
        product.Status.Should().Be(ProductStatus.Discontinued);

        ProductRepositoryMock.VerifyUpdateWasCalledWith(product);
    }

    [Fact]
    public async Task Handle_WithNonExistingProduct_ReturnsNotFoundWithoutPersisting()
    {
        // Arrange
        var externalId = Guid.NewGuid();
        ProductRepositoryMock.ConfigureFindByExternalIdToReturn(externalId, null);

        // Act
        var result = await Handler.Handle(new DiscontinueProductCommand(externalId), default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be("PRODUCT_NOT_FOUND");

        ProductRepositoryMock.VerifyUpdateWasNotCalled();
    }

    [Fact]
    public async Task Handle_WithAlreadyDiscontinuedProduct_ThrowsWithoutPersisting()
    {
        // Arrange
        var product = ProductFaker.Discontinued();
        ProductRepositoryMock.ConfigureFindByExternalIdToReturn(product.ExternalId, product);

        // Act
        var act = async () => await Handler.Handle(new DiscontinueProductCommand(product.ExternalId), default);

        // Assert
        await act.Should().ThrowAsync<ProductAlreadyDiscontinuedException>();

        ProductRepositoryMock.VerifyUpdateWasNotCalled();
    }
}
