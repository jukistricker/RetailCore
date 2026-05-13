using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RetailCore.Infrastructure.Data.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttributeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.ProductAttributes)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentValue)
            .WithMany(x => x.ChildValues)
            .HasForeignKey(x => x.ParentValueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PriceAdjustment)
            .HasColumnType("decimal(18,2)");
    }
}