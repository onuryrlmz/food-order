using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class DeliveryAssignmentConfiguration : IEntityTypeConfiguration<DeliveryAssignment>
{
    public void Configure(EntityTypeBuilder<DeliveryAssignment> builder)
    {
        builder.ToTable("DeliveryAssignment").HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("Id").IsRequired();
        builder.Property(d => d.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(d => d.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(d => d.CourierId).HasColumnName("CourierId");
        builder.Property(d => d.CourierCompanyId).HasColumnName("CourierCompanyId");
        builder.Property(d => d.AgreementId).HasColumnName("AgreementId");
        builder.Property(d => d.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(d => d.AssignmentStrategyId).HasColumnName("AssignmentStrategyId").IsRequired();
        builder.Property(d => d.DeliveryFee).HasColumnName("DeliveryFee").HasPrecision(18, 2);
        builder.Property(d => d.DistanceKm).HasColumnName("DistanceKm").HasPrecision(10, 2);
        builder.Property(d => d.CustomerLatitude).HasColumnName("CustomerLatitude").HasPrecision(10, 7);
        builder.Property(d => d.CustomerLongitude).HasColumnName("CustomerLongitude").HasPrecision(10, 7);
        builder.Property(d => d.RestaurantLatitude).HasColumnName("RestaurantLatitude").HasPrecision(10, 7);
        builder.Property(d => d.RestaurantLongitude).HasColumnName("RestaurantLongitude").HasPrecision(10, 7);
        builder.Property(d => d.OfferedAt).HasColumnName("OfferedAt");
        builder.Property(d => d.AcceptedAt).HasColumnName("AcceptedAt");
        builder.Property(d => d.RejectedAt).HasColumnName("RejectedAt");
        builder.Property(d => d.PickedUpAt).HasColumnName("PickedUpAt");
        builder.Property(d => d.DeliveredAt).HasColumnName("DeliveredAt");
        builder.Property(d => d.CancelledAt).HasColumnName("CancelledAt");
        builder.Property(d => d.CancellationReason).HasColumnName("CancellationReason").HasMaxLength(500);
        builder.Property(d => d.ExpiresAt).HasColumnName("ExpiresAt");
        builder.Property(d => d.AttemptNumber).HasColumnName("AttemptNumber").IsRequired();
        builder.Property(d => d.RejectionReason).HasColumnName("RejectionReason").HasMaxLength(500);
        builder.Property(d => d.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(d => d.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(d => d.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(d => d.Order).WithMany().HasForeignKey(d => d.OrderId);
        builder.HasOne(d => d.CourierCompany).WithMany().HasForeignKey(d => d.CourierCompanyId);
        builder.HasOne(d => d.Agreement).WithMany().HasForeignKey(d => d.AgreementId);

        builder.HasIndex(d => d.OrderId);
        builder.HasIndex(d => d.CourierId);
        builder.HasIndex(d => d.StatusId);

        builder.HasQueryFilter(d => !d.DeletedDate.HasValue);
    }
}