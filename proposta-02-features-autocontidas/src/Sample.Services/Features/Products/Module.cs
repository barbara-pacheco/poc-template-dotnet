using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sample.Services.Features.Products.Domain;
using Sample.Services.Features.Products.Infrastructure;
using Sample.Services.Features.Products.UseCases.Create;
using Sample.Services.Features.Products.UseCases.Discontinue;
using Sample.Services.Features.Products.UseCases.GetById;
using Sample.Shared.Errors;
using Sample.Shared.Modules;

namespace Sample.Services.Features.Products;

/// <summary>
/// Porta de entrada da feature Products: o que ela registra no DI e as rotas
/// HTTP que expõe. Descoberto sozinho pelo AddModules/MapModules.
/// </summary>
public sealed class Module : IModule
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", CreateAsync)
            .WithTags("Products")
            .WithSummary("Cria um produto")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapGet("/products/{externalId:guid}", GetByIdAsync)
            .WithTags("Products")
            .WithSummary("Busca um produto pelo id")
            .Produces<GetProductByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("/products/{externalId:guid}/discontinuation", DiscontinueAsync)
            .WithTags("Products")
            .WithSummary("Descontinua um produto")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> CreateAsync(
        CreateProductCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            created => Results.Created((string?)null, created),
            errors => errors.ToHttpResult());
    }

    private static async Task<IResult> GetByIdAsync(
        Guid externalId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(externalId), cancellationToken);

        return result.Match(Results.Ok, errors => errors.ToHttpResult());
    }

    private static async Task<IResult> DiscontinueAsync(
        Guid externalId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DiscontinueProductCommand(externalId), cancellationToken);

        return result.Match(_ => Results.NoContent(), errors => errors.ToHttpResult());
    }
}
