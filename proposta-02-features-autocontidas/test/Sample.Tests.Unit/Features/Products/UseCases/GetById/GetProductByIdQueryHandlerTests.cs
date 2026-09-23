using AwesomeAssertions;
using ErrorOr;
using Sample.Services.Features.Products.UseCases.GetById;
using Sample.Tests.Unit.Features.Products.UseCases.GetById.Fakers;
using Sample.Tests.Unit.Features.Products.UseCases.GetById.Fixtures;

namespace Sample.Tests.Unit.Features.Products.UseCases.GetById;

public class GetProductByIdQueryHandlerTests : GetProductByIdQueryHandlerFixture
{
    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsProduct()
    {
        // Arrange
        var product = ProductFaker.Valid();
        ProductRepositoryMock.ConfigureFindByExternalIdToReturn(product.ExternalId, product);

        // Act
        var result = await Handler.Handle(new GetProductByIdQuery(product.ExternalId), default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ExternalId.Should().Be(product.ExternalId);
        result.Value.Name.Should().Be(product.Name);
        result.Value.Status.Should().Be(product.Status);
    }

    [Fact]
    public async Task Handle_WithNonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var externalId = Guid.NewGuid();
        ProductRepositoryMock.ConfigureFindByExternalIdToReturn(externalId, null);

        // Act
        var result = await Handler.Handle(new GetProductByIdQuery(externalId), default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be("PRODUCT_NOT_FOUND");
    }
}
