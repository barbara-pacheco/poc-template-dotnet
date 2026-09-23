using AwesomeAssertions;
using Sample.Services.Features.Products.Domain;
using Sample.Tests.Unit.Features.Products.UseCases.Create.Fakers;
using Sample.Tests.Unit.Features.Products.UseCases.Create.Fixtures;

namespace Sample.Tests.Unit.Features.Products.UseCases.Create;

public class CreateProductCommandHandlerTests : CreateProductCommandHandlerFixture
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesProductAndPersists()
    {
        // Arrange
        var command = CreateProductCommandFaker.Valid();

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ExternalId.Should().NotBeEmpty();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Status.Should().Be(ProductStatus.Active);

        ProductRepositoryMock.VerifyAddWasCalledWith(command.Name);
    }

    [Fact]
    public async Task Handle_WithNameSurroundedBySpaces_TrimsName()
    {
        // Arrange
        var command = CreateProductCommandFaker.Valid();
        var paddedCommand = command with { Name = $"  {command.Name}  " };

        // Act
        var result = await Handler.Handle(paddedCommand, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(command.Name);

        ProductRepositoryMock.VerifyAddWasCalledWith(command.Name);
    }
}
