using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Domain.Entities;

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

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        
        builder.ToTable(t => t.HasCheckConstraint("CK_OrderItems_UnitPrice", "UnitPrice >= 0"));

        builder.Property(oi => oi.SubTotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.CreatedDate)
            .IsRequired();

        builder.Property(oi => oi.CreatedBy)
            .IsRequired();

        // FK → Orders
        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // FK → Products
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("OrderItems");
    }
}