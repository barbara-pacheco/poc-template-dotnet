using ErrorOr;
using Mediator;

namespace Sample.Application.Products.GetById;

public sealed record GetProductByIdQuery(Guid ExternalId) : IQuery<ErrorOr<GetProductByIdResponse>>;
