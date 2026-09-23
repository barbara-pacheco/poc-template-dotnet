using Mediator;
using Sample.Application.Products.Discontinue;
using Sample.WebApi._Shared;

namespace Sample.WebApi.Products.Discontinue;

public sealed class DiscontinueProductEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/products/{externalId:guid}/discontinuation", HandleAsync)
            .WithTags("Products")
            .WithSummary("Descontinua um produto")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid externalId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DiscontinueProductCommand(externalId), cancellationToken);

        return result.Match(_ => Results.NoContent(), errors => errors.ToHttpResult());
    }
}
