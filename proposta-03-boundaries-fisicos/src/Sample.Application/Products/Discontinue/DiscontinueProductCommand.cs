using ErrorOr;
using Mediator;

namespace Sample.Application.Products.Discontinue;

public sealed record DiscontinueProductCommand(Guid ExternalId) : ICommand<ErrorOr<Success>>;
