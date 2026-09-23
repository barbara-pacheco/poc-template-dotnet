using ErrorOr;
using Mediator;

namespace Sample.Application.Products.Create;

public sealed record CreateProductCommand(string Name) : ICommand<ErrorOr<CreateProductResponse>>;
