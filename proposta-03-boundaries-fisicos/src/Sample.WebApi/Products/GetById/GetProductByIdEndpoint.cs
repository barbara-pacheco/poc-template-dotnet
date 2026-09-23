using Mediator;
using Sample.Application.Products.GetById;
using Sample.WebApi._Shared;

namespace Sample.WebApi.Products.GetById;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/products/{externalId:guid}", HandleAsync)
            .WithTags("Products")
            .WithSummary("Busca um produto pelo id")
            .Produces<GetProductByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid externalId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(externalId), cancellationToken);

        return result.Match(Results.Ok, errors => errors.ToHttpResult());
    }
}
