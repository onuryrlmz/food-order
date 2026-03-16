using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class RestaurantCourierConfiguration : IEntityTypeConfiguration<RestaurantCourier>
{
    public void Configure(EntityTypeBuilder<RestaurantCourier> builder)
    {
        builder.ToTable("RestaurantCourier").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.RestaurantId, c.CourierId, c.StatusId });

        builder.HasOne(c => c.CourierUser)
            .WithMany()
            .HasForeignKey(c => c.CourierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
