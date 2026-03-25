using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("Restaurant").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();
        builder.Property(b => b.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(b => b.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(b => b.Phone).HasColumnName("Phone").HasMaxLength(20).IsRequired();
        builder.Property(b => b.Email).HasColumnName("Email").HasMaxLength(150);
        builder.Property(b => b.MinimumOrderPrice).HasColumnName("MinimumOrderPrice").HasPrecision(18, 2);
        builder.Property(b => b.MinDeliveryTime).HasColumnName("MinDeliveryTime");
        builder.Property(b => b.MaxDeliveryTime).HasColumnName("MaxDeliveryTime");
        builder.Property(b => b.CoverImage).HasColumnName("CoverImage").HasMaxLength(500);
        builder.Property(b => b.Description).HasColumnName("Description").HasMaxLength(1000);
        builder.Property(b => b.Latitude).HasColumnName("Latitude").HasPrecision(10, 7);
        builder.Property(b => b.Longitude).HasColumnName("Longitude").HasPrecision(10, 7);
        builder.Property(b => b.ServiceAreaPolygonWkt).HasColumnName("ServiceAreaPolygonWkt").HasColumnType("LONGTEXT");
        builder.Property(b => b.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(b => b.IsOpen).HasColumnName("IsOpen").IsRequired();
        builder.Property(b => b.Rating).HasColumnName("Rating").HasPrecision(3, 2);
        builder.Property(b => b.RatingCount).HasColumnName("RatingCount");
        builder.Property(b => b.DefaultAssignmentStrategyId).HasColumnName("DefaultAssignmentStrategyId");
        builder.Property(b => b.HasOwnCouriers).HasColumnName("HasOwnCouriers").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasMany(r => r.Subscriptions).WithOne(s => s.Restaurant).HasForeignKey(s => s.RestaurantId);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}