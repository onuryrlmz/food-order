using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierLocationConfiguration : IEntityTypeConfiguration<CourierLocation>
{
    public void Configure(EntityTypeBuilder<CourierLocation> builder)
    {
        builder.ToTable("CourierLocation").HasKey(c => c.Id);
        builder.HasIndex(c => c.CourierId);
        builder.HasIndex(c => c.OrderId);
        builder.HasOne(c => c.CourierUser)
            .WithMany()
            .HasForeignKey(c => c.CourierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
