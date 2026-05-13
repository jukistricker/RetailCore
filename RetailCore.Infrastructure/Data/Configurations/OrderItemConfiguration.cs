using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RetailCore.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_OrderItems_Quantity", "Quantity > 0"));

        builder.Property(oi => oi.ProductAttributeId)
            .IsRequired(false);

        builder.Property(oi => oi.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.ToTable(t => t.HasCheckConstraint("CK_OrderItems_Price", "Price >= 0"));


        builder.Property(oi => oi.CreatedDate)
            .IsRequired();

        builder.Property(oi => oi.CreatedBy)
            .IsRequired();

        // FK -> Orders
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK -> Products
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("OrderItems");
    }
}