using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class SettlementItemConfiguration : IEntityTypeConfiguration<SettlementItem>
{
    public void Configure(EntityTypeBuilder<SettlementItem> builder)
    {
        builder.ToTable("SettlementItem").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(e => e.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(e => e.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(e => e.OrderAmount).HasColumnName("OrderAmount").HasPrecision(18, 2).IsRequired();
        // Uygulanan oranın snapshot'ı (0–1 kesir), para değil — 6 ondalık ile saklanır.
        builder.Property(e => e.CommissionRate).HasColumnName("CommissionRate").HasPrecision(18, 6).IsRequired();
        builder.Property(e => e.CommissionAmount).HasColumnName("CommissionAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.FixedFee).HasColumnName("FixedFee").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.NetAmount).HasColumnName("NetAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.CommissionSourceType).HasColumnName("CommissionSourceType").IsRequired();
        builder.Property(e => e.CommissionSourceId).HasColumnName("CommissionSourceId").IsRequired();
        builder.Property(e => e.PeriodDate).HasColumnName("PeriodDate").IsRequired();
        builder.Property(e => e.SettlementPeriodId).HasColumnName("SettlementPeriodId");
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.SettlementPeriod).WithMany(p => p.Items).HasForeignKey(e => e.SettlementPeriodId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.SellerId);
        builder.HasIndex(e => e.RestaurantId);
        builder.HasIndex(e => e.SettlementPeriodId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
