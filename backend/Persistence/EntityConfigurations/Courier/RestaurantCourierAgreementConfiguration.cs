using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class RestaurantCourierAgreementConfiguration : IEntityTypeConfiguration<RestaurantCourierAgreement>
{
    public void Configure(EntityTypeBuilder<RestaurantCourierAgreement> builder)
    {
        builder.ToTable("RestaurantCourierAgreement").HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnName("Id").IsRequired();
        builder.Property(a => a.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(a => a.CourierCompanyId).HasColumnName("CourierCompanyId");
        builder.Property(a => a.CourierId).HasColumnName("CourierId");
        builder.Property(a => a.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(a => a.AssignmentStrategyId).HasColumnName("AssignmentStrategyId").IsRequired();
        builder.Property(a => a.AgreedDeliveryFee).HasColumnName("AgreedDeliveryFee").HasPrecision(18, 2);
        builder.Property(a => a.PerKmFee).HasColumnName("PerKmFee").HasPrecision(18, 2);
        builder.Property(a => a.Priority).HasColumnName("Priority").IsRequired();
        builder.Property(a => a.IsDefault).HasColumnName("IsDefault").IsRequired();
        builder.Property(a => a.EffectiveFrom).HasColumnName("EffectiveFrom");
        builder.Property(a => a.EffectiveUntil).HasColumnName("EffectiveUntil");
        builder.Property(a => a.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(a => a.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(a => a.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(a => a.Restaurant).WithMany().HasForeignKey(a => a.RestaurantId);
        builder.HasOne(a => a.Courier).WithMany().HasForeignKey(a => a.CourierId);

        builder.HasIndex(a => a.RestaurantId);

        builder.HasQueryFilter(a => !a.DeletedDate.HasValue);
    }
}