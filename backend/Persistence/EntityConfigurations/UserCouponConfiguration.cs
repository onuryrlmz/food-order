using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserCouponConfiguration : IEntityTypeConfiguration<UserCoupon>
{
    public void Configure(EntityTypeBuilder<UserCoupon> builder)
    {
        builder.ToTable("UserCoupon").HasKey(uc => uc.Id);

        builder.Property(uc => uc.Id).HasColumnName("Id").IsRequired();
        builder.Property(uc => uc.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(uc => uc.CouponId).HasColumnName("CouponId").IsRequired();
        builder.Property(uc => uc.OrderId).HasColumnName("OrderId");
        builder.Property(uc => uc.UsedAt).HasColumnName("UsedAt");
        builder.Property(uc => uc.UsageCount).HasColumnName("UsageCount").IsRequired().HasDefaultValue(0);
        builder.Property(uc => uc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(uc => uc.UpdatedDate).HasColumnName("UpdatedDate");

        builder.HasIndex(uc => new { uc.UserId, uc.CouponId });

        builder.HasOne(uc => uc.User).WithMany().HasForeignKey(uc => uc.UserId);
        builder.HasOne(uc => uc.Coupon).WithMany().HasForeignKey(uc => uc.CouponId);
        builder.HasOne(uc => uc.Order).WithMany().HasForeignKey(uc => uc.OrderId);
    }
}
