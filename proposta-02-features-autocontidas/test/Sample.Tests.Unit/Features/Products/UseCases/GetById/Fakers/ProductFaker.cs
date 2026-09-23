using Bogus;
using Sample.Services.Features.Products.Domain;

namespace Sample.Tests.Unit.Features.Products.UseCases.GetById.Fakers;

public static class ProductFaker
{
    public static Product Valid()
    {
        return new Faker<Product>()
            .CustomInstantiator(f => Product.Create(f.Commerce.ProductName()))
            .Generate();
    }
}
