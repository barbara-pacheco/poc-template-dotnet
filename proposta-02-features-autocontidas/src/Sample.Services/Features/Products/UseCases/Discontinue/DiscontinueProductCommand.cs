using ErrorOr;
using Mediator;

namespace Sample.Services.Features.Products.UseCases.Discontinue;

public sealed record DiscontinueProductCommand(Guid ExternalId) : ICommand<ErrorOr<Success>>;
