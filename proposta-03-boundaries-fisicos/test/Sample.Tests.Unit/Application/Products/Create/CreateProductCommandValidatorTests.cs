using AwesomeAssertions;
using Sample.Application.Products.Create;

namespace Sample.Tests.Unit.Application.Products.Create;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        // Arrange
        var command = new CreateProductCommand("Caneta");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyName_HasErrorForName(string name)
    {
        // Arrange
        var command = new CreateProductCommand(name);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateProductCommand.Name) &&
            e.ErrorMessage == "O nome do produto é obrigatório.");
    }

    [Fact]
    public void Validate_WithNameAboveMaxLength_HasErrorForName()
    {
        // Arrange
        var command = new CreateProductCommand(new string('a', CreateProductCommandValidator.NameMaxLength + 1));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateProductCommand.Name) &&
            e.ErrorMessage == $"O nome do produto deve ter no máximo {CreateProductCommandValidator.NameMaxLength} caracteres.");
    }

    [Fact]
    public void Validate_WithNameAtMaxLength_IsValid()
    {
        // Arrange
        var command = new CreateProductCommand(new string('a', CreateProductCommandValidator.NameMaxLength));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
