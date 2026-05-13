using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // FK -> ProductAttributes
        builder.HasOne(ci => ci.ProductAttribute)
            .WithMany()
            .HasForeignKey(ci => ci.ProductAttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ci => ci.CustomerId)
            .HasDatabaseName("IX_CartItems_CustomerId");

        builder.ToTable("CartItems");
    }
}