using Bogus;
using Sample.Domain.Products;

namespace Sample.Tests.Unit.Application.Products.GetById.Fakers;

public static class ProductFaker
{
    public static Product Valid()
    {
        return new Faker<Product>()
            .CustomInstantiator(f => Product.Create(f.Commerce.ProductName()))
            .Generate();
    }
}
