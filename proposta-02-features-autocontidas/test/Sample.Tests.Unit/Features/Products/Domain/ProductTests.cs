using AwesomeAssertions;
using Sample.Services.Features.Products.Domain;
using Sample.Tests.Unit.Features.Products.Domain.Fakers;

namespace Sample.Tests.Unit.Features.Products.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithValidName_CreatesActiveProduct()
    {
        // Act
        var product = Product.Create("Caneta");

        // Assert
        product.ExternalId.Should().NotBeEmpty();
        product.Name.Should().Be("Caneta");
        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Create_TwoProducts_GetDifferentExternalIds()
    {
        // Act
        var first = Product.Create("Caneta");
        var second = Product.Create("Caneta");

        // Assert
        first.ExternalId.Should().NotBe(second.ExternalId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsInvalidProductException(string name)
    {
        // Arrange
        var act = () => Product.Create(name);

        // Act & Assert
        act.Should().Throw<InvalidProductException>()
            .WithMessage("O nome do produto não pode ser vazio.");
    }

    [Fact]
    public void Discontinue_ActiveProduct_BecomesDiscontinued()
    {
        // Arrange
        var product = ProductFaker.Valid();

        // Act
        product.Discontinue();

        // Assert
        product.Status.Should().Be(ProductStatus.Discontinued);
    }

    [Fact]
    public void Discontinue_AlreadyDiscontinuedProduct_ThrowsProductAlreadyDiscontinuedException()
    {
        // Arrange
        var product = ProductFaker.Discontinued();
        var act = () => product.Discontinue();

        // Act & Assert
        act.Should().Throw<ProductAlreadyDiscontinuedException>();
    }
}
