using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Common;

public class PlatformCommissionScheduleConfiguration : IEntityTypeConfiguration<PlatformCommissionSchedule>
{
    public void Configure(EntityTypeBuilder<PlatformCommissionSchedule> builder)
    {
        builder.ToTable("PlatformCommissionSchedule").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.CommissionRate).HasColumnName("CommissionRate").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.FixedFee).HasColumnName("FixedFee").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.EffectiveFrom).HasColumnName("EffectiveFrom").IsRequired();
        builder.Property(e => e.EffectiveTo).HasColumnName("EffectiveTo");
        builder.Property(e => e.SetByUserId).HasColumnName("SetByUserId");
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
