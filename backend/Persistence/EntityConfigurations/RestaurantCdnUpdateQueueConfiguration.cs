using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RestaurantCdnUpdateQueueConfiguration : IEntityTypeConfiguration<RestaurantCdnUpdateQueue>
{
    public void Configure(EntityTypeBuilder<RestaurantCdnUpdateQueue> builder)
    {
        builder.ToTable("RestaurantCdnUpdateQueue").HasKey(q => q.Id);

        builder.Property(q => q.Id).HasColumnName("Id").IsRequired();
        builder.Property(q => q.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(q => q.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(q => q.ErrorMessage).HasColumnName("ErrorMessage").HasMaxLength(1000);
        builder.Property(q => q.ProcessedAt).HasColumnName("ProcessedAt");
        builder.Property(q => q.RetryCount).HasColumnName("RetryCount").IsRequired();
        builder.Property(q => q.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(q => q.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(q => q.DeletedDate).HasColumnName("DeletedDate");

        // Pending işler için hızlı sorgulama
        builder.HasIndex(q => new { q.StatusId, q.CreatedDate });
    }
}