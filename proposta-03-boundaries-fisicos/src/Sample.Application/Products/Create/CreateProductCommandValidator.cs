using FluentValidation;

namespace Sample.Application.Products.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public const int NameMaxLength = 120;

    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"O nome do produto deve ter no máximo {NameMaxLength} caracteres.");
    }
}
