using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RestaurantWorkingHourConfiguration : IEntityTypeConfiguration<RestaurantWorkingHour>
{
    public void Configure(EntityTypeBuilder<RestaurantWorkingHour> builder)
    {
        builder.ToTable("RestaurantWorkingHour");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DayOfWeek).IsRequired();
        builder.Property(x => x.OpenTime).IsRequired();
        builder.Property(x => x.CloseTime).IsRequired();
        builder.HasIndex(x => new { x.RestaurantId, x.DayOfWeek });
        builder.HasQueryFilter(x => x.DeletedDate == null);
    }
}
