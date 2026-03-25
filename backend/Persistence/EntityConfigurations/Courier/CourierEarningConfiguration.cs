using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierEarningConfiguration : IEntityTypeConfiguration<CourierEarning>
{
    public void Configure(EntityTypeBuilder<CourierEarning> builder)
    {
        builder.ToTable("CourierEarning").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.CourierId).HasColumnName("CourierId").IsRequired();
        builder.Property(e => e.DeliveryAssignmentId).HasColumnName("DeliveryAssignmentId").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(e => e.DeliveryFee).HasColumnName("DeliveryFee").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TipAmount).HasColumnName("TipAmount").HasPrecision(18, 2);
        builder.Property(e => e.BonusAmount).HasColumnName("BonusAmount").HasPrecision(18, 2);
        builder.Property(e => e.TotalEarning).HasColumnName("TotalEarning").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.IsSettled).HasColumnName("IsSettled").IsRequired();
        builder.Property(e => e.SettledAt).HasColumnName("SettledAt");
        builder.Property(e => e.SettlementReference).HasColumnName("SettlementReference").HasMaxLength(200);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.DeliveryAssignment).WithMany().HasForeignKey(e => e.DeliveryAssignmentId);

        builder.HasIndex(e => e.CourierId);
        builder.HasIndex(e => e.IsSettled);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
