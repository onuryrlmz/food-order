using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierConfiguration : IEntityTypeConfiguration<Domain.Entities.Courier.Courier>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Courier.Courier> builder)
    {
        builder.ToTable("Courier").HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("Id").IsRequired();
        builder.Property(c => c.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(c => c.CourierCompanyId).HasColumnName("CourierCompanyId");
        builder.Property(c => c.RestaurantId).HasColumnName("RestaurantId");
        builder.Property(c => c.CourierTypeId).HasColumnName("CourierTypeId").IsRequired();
        builder.Property(c => c.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(c => c.AvailabilityStatusId).HasColumnName("AvailabilityStatusId").IsRequired();
        builder.Property(c => c.VehicleType).HasColumnName("VehicleType").HasMaxLength(50);
        builder.Property(c => c.VehiclePlate).HasColumnName("VehiclePlate").HasMaxLength(20);
        builder.Property(c => c.IdentityNumber).HasColumnName("IdentityNumber").HasMaxLength(20);
        builder.Property(c => c.IBAN).HasColumnName("IBAN").HasMaxLength(34);
        builder.Property(c => c.Rating).HasColumnName("Rating").HasPrecision(3, 2).IsRequired();
        builder.Property(c => c.RatingCount).HasColumnName("RatingCount").IsRequired();
        builder.Property(c => c.TotalDeliveries).HasColumnName("TotalDeliveries").IsRequired();
        builder.Property(c => c.CurrentLatitude).HasColumnName("CurrentLatitude").HasPrecision(10, 7);
        builder.Property(c => c.CurrentLongitude).HasColumnName("CurrentLongitude").HasPrecision(10, 7);
        builder.Property(c => c.LastLocationUpdate).HasColumnName("LastLocationUpdate");
        builder.Property(c => c.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(c => c.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(c => c.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId);
        builder.HasOne(c => c.Restaurant).WithMany().HasForeignKey(c => c.RestaurantId);
        builder.HasMany(c => c.DeliveryAssignments).WithOne(d => d.Courier).HasForeignKey(d => d.CourierId);
        builder.HasMany(c => c.Earnings).WithOne(e => e.Courier).HasForeignKey(e => e.CourierId);

        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.AvailabilityStatusId);

        builder.HasQueryFilter(c => !c.DeletedDate.HasValue);
    }
}
