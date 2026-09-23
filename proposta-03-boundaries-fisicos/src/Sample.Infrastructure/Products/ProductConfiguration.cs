using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Domain.Products;

namespace Sample.Infrastructure.Products;

/// <summary>
/// Mapeamento Product ↔ tabela "product". Descoberto sozinho pelo
/// ApplyConfigurationsFromAssembly do SampleDbContext — não precisa
/// registrar em lugar nenhum. Nomes de coluna saem em snake_case pela
/// convenção global (UseSnakeCaseNamingConvention), não daqui.
/// </summary>
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public const int NameMaxLength = 120;

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("product");

        builder.Property<Guid>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.Property(product => product.ExternalId).IsRequired();
        builder.HasIndex(product => product.ExternalId).IsUnique();

        builder.Property(product => product.Name).IsRequired().HasMaxLength(NameMaxLength);

        builder.Property(product => product.Status).IsRequired().HasConversion<string>().HasMaxLength(32);

        builder.Property<DateTimeOffset>("CreatedAt").HasDefaultValueSql("now()");
    }
}
