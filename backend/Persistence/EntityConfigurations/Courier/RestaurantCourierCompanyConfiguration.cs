using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class RestaurantCourierCompanyConfiguration : IEntityTypeConfiguration<RestaurantCourierCompany>
{
    public void Configure(EntityTypeBuilder<RestaurantCourierCompany> builder)
    {
        builder.ToTable("RestaurantCourierCompany").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.RestaurantId, c.CourierCompanyId, c.StatusId });

        builder.HasOne(c => c.Restaurant)
            .WithMany()
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CourierCompany)
            .WithMany()
            .HasForeignKey(c => c.CourierCompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
