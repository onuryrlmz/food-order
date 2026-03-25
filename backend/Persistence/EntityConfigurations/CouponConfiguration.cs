using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupon").HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("Id").IsRequired();
        builder.Property(c => c.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(c => c.RestaurantId).HasColumnName("RestaurantId");
        builder.Property(c => c.Code).HasColumnName("Code").HasMaxLength(50).IsRequired();
        builder.Property(c => c.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasColumnName("Description").HasMaxLength(500);
        builder.Property(c => c.Type).HasColumnName("Type").IsRequired();
        builder.Property(c => c.Value).HasColumnName("Value").HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.MaxDiscountAmount).HasColumnName("MaxDiscountAmount").HasPrecision(18, 2);
        builder.Property(c => c.BuyQuantity).HasColumnName("BuyQuantity").IsRequired();
        builder.Property(c => c.GetQuantity).HasColumnName("GetQuantity").IsRequired();
        builder.Property(c => c.MinOrderAmount).HasColumnName("MinOrderAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.ApplicableType).HasColumnName("ApplicableType").IsRequired();
        builder.Property(c => c.StartDate).HasColumnName("StartDate").IsRequired();
        builder.Property(c => c.EndDate).HasColumnName("EndDate").IsRequired();
        builder.Property(c => c.UsageLimit).HasColumnName("UsageLimit");
        builder.Property(c => c.UsagePerUser).HasColumnName("UsagePerUser");
        builder.Property(c => c.CurrentUsageCount).HasColumnName("CurrentUsageCount").IsRequired().HasDefaultValue(0);
        builder.Property(c => c.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(c => c.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(c => c.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.SellerId);
        builder.HasIndex(c => c.RestaurantId);

        builder.HasOne(c => c.Seller).WithMany().HasForeignKey(c => c.SellerId);
        builder.HasOne(c => c.Restaurant).WithMany().HasForeignKey(c => c.RestaurantId);
        builder.HasMany(c => c.CouponMenus).WithOne(cm => cm.Coupon).HasForeignKey(cm => cm.CouponId);
        builder.HasMany(c => c.CouponCategories).WithOne(cc => cc.Coupon).HasForeignKey(cc => cc.CouponId);

        builder.HasQueryFilter(c => !c.DeletedDate.HasValue);
    }
}