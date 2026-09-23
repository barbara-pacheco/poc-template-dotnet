using Bogus;
using Sample.Domain.Products;

namespace Sample.Tests.Unit.Application.Products.Discontinue.Fakers;

public static class ProductFaker
{
    public static Product Valid()
    {
        return new Faker<Product>()
            .CustomInstantiator(f => Product.Create(f.Commerce.ProductName()))
            .Generate();
    }

    public static Product Discontinued()
    {
        var product = Valid();
        product.Discontinue();
        return product;
    }
}
