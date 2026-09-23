using Mediator;
using Sample.Application.Products.Create;
using Sample.WebApi._Shared;

namespace Sample.WebApi.Products.Create;

public sealed class CreateProductEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/products", HandleAsync)
            .WithTags("Products")
            .WithSummary("Cria um produto")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        CreateProductCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            created => Results.Created((string?)null, created),
            errors => errors.ToHttpResult());
    }
}
