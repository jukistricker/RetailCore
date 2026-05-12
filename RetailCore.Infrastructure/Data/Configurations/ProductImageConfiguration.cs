using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Domain.Entities;

namespace RetailCore.Infrastructure.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pi => pi.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pi => pi.AltText)
            .HasMaxLength(200);

        builder.Property(pi => pi.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pi => pi.CreatedDate)
            .IsRequired();

        builder.Property(pi => pi.CreatedBy)
            .IsRequired();

        // FK -> Products
        builder.HasOne<Product>()
            .WithMany(p=>p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pi => pi.ProductId)
            .HasDatabaseName("IX_ProductImages_ProductId");

        builder.ToTable("ProductImages");
    }
}