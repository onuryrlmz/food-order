using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Common;

public class UserExternalInfoConfiguration : IEntityTypeConfiguration<UserExternalInfo>
{
    public void Configure(EntityTypeBuilder<UserExternalInfo> builder)
    {
        builder.ToTable("UserExternalInfo").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.Provider).HasColumnName("Provider").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Key).HasColumnName("Key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Value).HasColumnName("Value").HasMaxLength(500).IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Provider, e.Key }).IsUnique();

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
