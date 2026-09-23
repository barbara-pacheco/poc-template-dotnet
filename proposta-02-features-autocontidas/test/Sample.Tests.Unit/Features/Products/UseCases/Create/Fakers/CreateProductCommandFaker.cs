using Bogus;
using Sample.Services.Features.Products.UseCases.Create;

namespace Sample.Tests.Unit.Features.Products.UseCases.Create.Fakers;

public static class CreateProductCommandFaker
{
    public static CreateProductCommand Valid()
    {
        return new Faker<CreateProductCommand>()
            .CustomInstantiator(f => new CreateProductCommand(f.Commerce.ProductName()))
            .Generate();
    }
}
