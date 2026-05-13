using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RetailCore.Infrastructure.Data.Configurations;

public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pr => pr.Rating)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_ProductRatings_Rating", "Rating BETWEEN 1 AND 5"));

        builder.Property(pr => pr.Review)
            .HasMaxLength(1000);

        builder.Property(pr => pr.CreatedDate)
            .IsRequired();

        builder.Property(pr => pr.CreatedBy)
            .IsRequired();

        // FK → Products
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(pr => pr.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // FK → Customers
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(pr => pr.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // 1 customer chỉ rate 1 product 1 lần
        builder.HasIndex(pr => new { pr.ProductId, pr.CustomerId })
            .IsUnique()
            .HasDatabaseName("UQ_ProductRatings_Product_Customer");

        builder.HasIndex(pr => pr.ProductId)
            .HasDatabaseName("IX_ProductRatings_ProductId");

        builder.ToTable("ProductRatings");
    }
}