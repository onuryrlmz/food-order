using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetToken").HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(p => p.Code).HasColumnName("Code").HasMaxLength(10).IsRequired();
        builder.Property(p => p.Method).HasColumnName("Method").IsRequired();
        builder.Property(p => p.ExpiresAt).HasColumnName("ExpiresAt").IsRequired();
        builder.Property(p => p.UsedAt).HasColumnName("UsedAt");
        builder.Property(p => p.FailedAttempts).HasColumnName("FailedAttempts").HasDefaultValue(0);
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(p => p.UserId);

        builder.HasQueryFilter(p => !p.DeletedDate.HasValue);
    }
}
