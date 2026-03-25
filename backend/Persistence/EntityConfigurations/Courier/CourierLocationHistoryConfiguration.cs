using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierLocationHistoryConfiguration : IEntityTypeConfiguration<CourierLocationHistory>
{
    public void Configure(EntityTypeBuilder<CourierLocationHistory> builder)
    {
        builder.ToTable("CourierLocationHistory").HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("Id").IsRequired();
        builder.Property(l => l.CourierId).HasColumnName("CourierId").IsRequired();
        builder.Property(l => l.DeliveryAssignmentId).HasColumnName("DeliveryAssignmentId");
        builder.Property(l => l.Latitude).HasColumnName("Latitude").HasPrecision(10, 7).IsRequired();
        builder.Property(l => l.Longitude).HasColumnName("Longitude").HasPrecision(10, 7).IsRequired();
        builder.Property(l => l.RecordedAt).HasColumnName("RecordedAt").IsRequired();
        builder.Property(l => l.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(l => l.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(l => l.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(l => l.Courier).WithMany().HasForeignKey(l => l.CourierId);
        builder.HasOne(l => l.DeliveryAssignment).WithMany().HasForeignKey(l => l.DeliveryAssignmentId);

        builder.HasIndex(l => l.CourierId);
        builder.HasIndex(l => l.RecordedAt);

        builder.HasQueryFilter(l => !l.DeletedDate.HasValue);
    }
}
