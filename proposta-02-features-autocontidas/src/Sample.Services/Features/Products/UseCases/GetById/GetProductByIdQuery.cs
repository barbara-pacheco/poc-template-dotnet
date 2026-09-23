using ErrorOr;
using Mediator;

namespace Sample.Services.Features.Products.UseCases.GetById;

public sealed record GetProductByIdQuery(Guid ExternalId) : IQuery<ErrorOr<GetProductByIdResponse>>;
