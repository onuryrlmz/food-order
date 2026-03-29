using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class SettlementPeriodConfiguration : IEntityTypeConfiguration<SettlementPeriod>
{
    public void Configure(EntityTypeBuilder<SettlementPeriod> builder)
    {
        builder.ToTable("SettlementPeriod").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(e => e.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(e => e.PeriodDate).HasColumnName("PeriodDate").IsRequired();
        builder.Property(e => e.TotalOrderCount).HasColumnName("TotalOrderCount").IsRequired();
        builder.Property(e => e.TotalOrderAmount).HasColumnName("TotalOrderAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TotalCommission).HasColumnName("TotalCommission").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TotalFixedFee).HasColumnName("TotalFixedFee").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TotalNetAmount).HasColumnName("TotalNetAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(e => e.IBAN).HasColumnName("IBAN").HasMaxLength(34);
        builder.Property(e => e.BankTransferRef).HasColumnName("BankTransferRef").HasMaxLength(200);
        builder.Property(e => e.ApprovedByUserId).HasColumnName("ApprovedByUserId");
        builder.Property(e => e.ApprovedAt).HasColumnName("ApprovedAt");
        builder.Property(e => e.PaidAt).HasColumnName("PaidAt");
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasMany(e => e.Items).WithOne(i => i.SettlementPeriod).HasForeignKey(i => i.SettlementPeriodId);

        builder.HasIndex(e => e.SellerId);
        builder.HasIndex(e => e.RestaurantId);
        builder.HasIndex(e => e.StatusId);
        builder.HasIndex(e => e.PeriodDate);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
