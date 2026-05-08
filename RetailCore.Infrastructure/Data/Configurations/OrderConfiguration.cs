using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Enums;

namespace RetailCore.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()    // lưu enum dạng string vào DB
            .HasMaxLength(20)
            .HasDefaultValue(OrderStatus.Pending);

        
        builder.ToTable(t => t.HasCheckConstraint("CK_Orders_Status",
            "Status IN ('Pending','Confirmed','Shipping','Delivered','Cancelled')"));

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        
        builder.ToTable(t => t.HasCheckConstraint("CK_Orders_TotalAmount", "TotalAmount >= 0"));

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.ShippingCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Note)
            .HasMaxLength(500);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .IsRequired();

        // FK → Customers
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("IX_Orders_CustomerId");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Orders_Status");

        builder.ToTable("Orders");
    }
}