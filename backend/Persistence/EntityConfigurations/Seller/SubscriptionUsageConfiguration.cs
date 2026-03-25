using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class SubscriptionUsageConfiguration : IEntityTypeConfiguration<SubscriptionUsage>
{
    public void Configure(EntityTypeBuilder<SubscriptionUsage> builder)
    {
        builder.ToTable("SubscriptionUsage").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.SubscriptionId, c.Year, c.Month }).IsUnique();

        builder.HasOne(c => c.Subscription)
            .WithMany()
            .HasForeignKey(c => c.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}