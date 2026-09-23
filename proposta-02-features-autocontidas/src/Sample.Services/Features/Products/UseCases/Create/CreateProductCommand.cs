using ErrorOr;
using Mediator;

namespace Sample.Services.Features.Products.UseCases.Create;

public sealed record CreateProductCommand(string Name) : ICommand<ErrorOr<CreateProductResponse>>;
