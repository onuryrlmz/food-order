using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class CouponMenuConfiguration : IEntityTypeConfiguration<CouponMenu>
{
    public void Configure(EntityTypeBuilder<CouponMenu> builder)
    {
        builder.ToTable("CouponMenu").HasKey(cm => new { cm.CouponId, cm.MenuId });

        builder.Property(cm => cm.CouponId).HasColumnName("CouponId").IsRequired();
        builder.Property(cm => cm.MenuId).HasColumnName("MenuId").IsRequired();

        builder.HasOne(cm => cm.Coupon).WithMany(c => c.CouponMenus).HasForeignKey(cm => cm.CouponId);
        builder.HasOne(cm => cm.Menu).WithMany().HasForeignKey(cm => cm.MenuId);
    }
}
