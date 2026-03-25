using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class CouponCategoryConfiguration : IEntityTypeConfiguration<CouponCategory>
{
    public void Configure(EntityTypeBuilder<CouponCategory> builder)
    {
        builder.ToTable("CouponCategory").HasKey(cc => new { cc.CouponId, cc.CategoryId });

        builder.Property(cc => cc.CouponId).HasColumnName("CouponId").IsRequired();
        builder.Property(cc => cc.CategoryId).HasColumnName("CategoryId").IsRequired();

        builder.HasOne(cc => cc.Coupon).WithMany(c => c.CouponCategories).HasForeignKey(cc => cc.CouponId);
        builder.HasOne(cc => cc.Category).WithMany().HasForeignKey(cc => cc.CategoryId);
    }
}