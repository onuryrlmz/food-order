using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscription").HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("Id").IsRequired();
        builder.Property(s => s.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(s => s.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(s => s.SubscriptionPlanId).HasColumnName("SubscriptionPlanId").IsRequired();
        builder.Property(s => s.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(s => s.StartDate).HasColumnName("StartDate").IsRequired();
        builder.Property(s => s.EndDate).HasColumnName("EndDate").IsRequired();
        builder.Property(s => s.PaidAmount).HasColumnName("PaidAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Notes).HasColumnName("Notes").HasMaxLength(500);
        builder.Property(s => s.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(s => s.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(s => !s.DeletedDate.HasValue);
    }
}
