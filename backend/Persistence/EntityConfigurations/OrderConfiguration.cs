using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order").HasKey(o => o.Id);

        builder.Property(o => o.Id).HasColumnName("Id").IsRequired();
        builder.Property(o => o.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(o => o.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(o => o.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(o => o.DeliveryAddressId).HasColumnName("DeliveryAddressId").IsRequired();
        builder.Property(o => o.InvoiceAddressId).HasColumnName("InvoiceAddressId");
        builder.Property(o => o.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(o => o.PaymentStatusId).HasColumnName("PaymentStatusId").IsRequired();
        builder.Property(o => o.PaymentOptionId).HasColumnName("PaymentOptionId").IsRequired();
        builder.Property(o => o.TotalProductPrice).HasColumnName("TotalProductPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.ShipmentPrice).HasColumnName("ShipmentPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.DiscountAmount).HasColumnName("DiscountAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.TotalPrice).HasColumnName("TotalPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.Notes).HasColumnName("Notes").HasMaxLength(500);
        builder.Property(o => o.CouponId).HasColumnName("CouponId");
        builder.Property(o => o.CouponCode).HasColumnName("CouponCode").HasMaxLength(50);
        builder.Property(o => o.CancellationReason).HasColumnName("CancellationReason").HasMaxLength(500);
        builder.Property(o => o.CourierId).HasColumnName("CourierId");
        builder.Property(o => o.DeliveryAssignmentId).HasColumnName("DeliveryAssignmentId");
        builder.Property(o => o.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(o => o.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(o => o.DeletedDate).HasColumnName("DeletedDate");

        builder.HasMany(o => o.OrderItems).WithOne(oi => oi.Order).HasForeignKey(oi => oi.OrderId);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.RestaurantId);
        builder.HasIndex(o => o.SellerId);
        builder.HasIndex(o => o.StatusId);

        builder.HasQueryFilter(o => !o.DeletedDate.HasValue);
    }
}