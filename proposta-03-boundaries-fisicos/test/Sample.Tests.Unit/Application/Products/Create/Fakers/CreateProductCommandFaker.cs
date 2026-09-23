using Bogus;
using Sample.Application.Products.Create;

namespace Sample.Tests.Unit.Application.Products.Create.Fakers;

public static class CreateProductCommandFaker
{
    public static CreateProductCommand Valid()
    {
        return new Faker<CreateProductCommand>()
            .CustomInstantiator(f => new CreateProductCommand(f.Commerce.ProductName()))
            .Generate();
    }
}
