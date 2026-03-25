using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class FavoriteRestaurantConfiguration : IEntityTypeConfiguration<FavoriteRestaurant>
{
    public void Configure(EntityTypeBuilder<FavoriteRestaurant> builder)
    {
        builder.ToTable("FavoriteRestaurant").HasKey(f => f.Id);

        builder.Property(f => f.Id).HasColumnName("Id").IsRequired();
        builder.Property(f => f.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(f => f.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(f => f.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(f => f.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(f => f.DeletedDate).HasColumnName("DeletedDate");

        // One favorite per user-restaurant pair
        builder.HasIndex(f => new { f.UserId, f.RestaurantId }).IsUnique();

        builder.HasQueryFilter(f => !f.DeletedDate.HasValue);
    }
}