using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class RestaurantCommissionConfiguration : IEntityTypeConfiguration<RestaurantCommission>
{
    public void Configure(EntityTypeBuilder<RestaurantCommission> builder)
    {
        builder.ToTable("RestaurantCommission").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        // Oran (0–1 arası kesir), para değil — kesirli oranların yuvarlanmaması için 6 ondalık.
        builder.Property(e => e.CommissionRate).HasColumnName("CommissionRate").HasPrecision(18, 6).IsRequired();
        builder.Property(e => e.FixedFee).HasColumnName("FixedFee").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.EffectiveFrom).HasColumnName("EffectiveFrom").IsRequired();
        builder.Property(e => e.EffectiveTo).HasColumnName("EffectiveTo");
        builder.Property(e => e.SetByUserId).HasColumnName("SetByUserId");
        builder.Property(e => e.Reason).HasColumnName("Reason").HasMaxLength(500);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Restaurant).WithMany().HasForeignKey(e => e.RestaurantId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.RestaurantId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
