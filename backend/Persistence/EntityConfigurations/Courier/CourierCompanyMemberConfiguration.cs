using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierCompanyMemberConfiguration : IEntityTypeConfiguration<CourierCompanyMember>
{
    public void Configure(EntityTypeBuilder<CourierCompanyMember> builder)
    {
        builder.ToTable("CourierCompanyMember").HasKey(c => c.Id);
        builder.HasIndex(c => new { c.CourierCompanyId, c.CourierId, c.StatusId });

        builder.HasOne(c => c.CourierCompany)
            .WithMany(cc => cc.Members)
            .HasForeignKey(c => c.CourierCompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CourierUser)
            .WithMany()
            .HasForeignKey(c => c.CourierId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
