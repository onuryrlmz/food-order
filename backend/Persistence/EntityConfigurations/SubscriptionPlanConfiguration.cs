using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlan").HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id).HasColumnName("Id").IsRequired();
        builder.Property(sp => sp.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
        builder.Property(sp => sp.Description).HasColumnName("Description").HasMaxLength(500);
        builder.Property(sp => sp.PlanType).HasColumnName("PlanType").IsRequired();
        builder.Property(sp => sp.MonthlyPrice).HasColumnName("MonthlyPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(sp => sp.MaxRestaurants).HasColumnName("MaxRestaurants").IsRequired();
        builder.Property(sp => sp.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(sp => sp.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(sp => sp.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(sp => sp.DeletedDate).HasColumnName("DeletedDate");

        builder.HasMany(sp => sp.Subscriptions).WithOne(s => s.SubscriptionPlan).HasForeignKey(s => s.SubscriptionPlanId);

        builder.HasQueryFilter(sp => !sp.DeletedDate.HasValue);
    }
}