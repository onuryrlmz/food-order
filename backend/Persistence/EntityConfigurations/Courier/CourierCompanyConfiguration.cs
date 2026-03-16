using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierCompanyConfiguration : IEntityTypeConfiguration<CourierCompany>
{
    public void Configure(EntityTypeBuilder<CourierCompany> builder)
    {
        builder.ToTable("CourierCompany").HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ContactEmail).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.OwnerUserId).IsUnique();

        builder.HasOne(c => c.OwnerUser)
            .WithMany()
            .HasForeignKey(c => c.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
