using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RetailCore.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("UQ_Products_Slug");

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.ToTable(t => t.HasCheckConstraint("CK_Products_Price", "Price >= 0"));

        builder.Property(p => p.Stock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.ToTable(t => t.HasCheckConstraint("CK_Products_Stock", "Stock >= 0"));

        builder.Property(p => p.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(p => p.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedDate)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .IsRequired();

        // FK -> Categories
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.IsFeatured)
            .HasFilter("IsFeatured = 1")
            .HasDatabaseName("IX_Products_IsFeatured");

        builder.ToTable("Products");
    }
}