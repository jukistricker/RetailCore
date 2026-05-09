using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Domain.Entities;

namespace RetailCore.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ci => ci.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.ToTable(t => t.HasCheckConstraint("CK_CartItems_Quantity", "Quantity > 0"));

        builder.Property(ci => ci.CreatedDate)
            .IsRequired();

        builder.Property(ci => ci.CreatedBy)
            .IsRequired();

        // FK -> Customers
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(ci => ci.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // FK -> Products
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // 1 customer chỉ add 1 product 1 lần
        builder.HasIndex(ci => new { ci.CustomerId, ci.ProductId })
            .IsUnique()
            .HasDatabaseName("UQ_CartItems_Customer_Product");

        builder.HasIndex(ci => ci.CustomerId)
            .HasDatabaseName("IX_CartItems_CustomerId");

        builder.ToTable("CartItems");
    }
}