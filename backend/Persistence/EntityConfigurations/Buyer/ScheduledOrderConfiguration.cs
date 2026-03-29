using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class ScheduledOrderConfiguration : IEntityTypeConfiguration<ScheduledOrder>
{
    public void Configure(EntityTypeBuilder<ScheduledOrder> builder)
    {
        builder.ToTable("ScheduledOrder").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(e => e.DeliveryAddressId).HasColumnName("DeliveryAddressId").IsRequired();
        builder.Property(e => e.InvoiceAddressId).HasColumnName("InvoiceAddressId");
        builder.Property(e => e.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(e => e.ScheduledDeliveryTime).HasColumnName("ScheduledDeliveryTime").IsRequired();
        builder.Property(e => e.ProcessAt).HasColumnName("ProcessAt").IsRequired();
        builder.Property(e => e.PaymentOptionId).HasColumnName("PaymentOptionId").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(500);
        builder.Property(e => e.CancellationReason).HasColumnName("CancellationReason").HasMaxLength(500);
        builder.Property(e => e.ConvertedOrderId).HasColumnName("ConvertedOrderId");
        builder.Property(e => e.BasketSnapshotJson).HasColumnName("BasketSnapshotJson").IsRequired();
        builder.Property(e => e.CouponId).HasColumnName("CouponId");
        builder.Property(e => e.CouponCode).HasColumnName("CouponCode").HasMaxLength(50);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.RestaurantId);
        builder.HasIndex(e => e.StatusId);
        builder.HasIndex(e => e.ProcessAt);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
