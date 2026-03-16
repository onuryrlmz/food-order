using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken").HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("Id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(r => r.Token).HasColumnName("Token").HasMaxLength(256).IsRequired();
        builder.Property(r => r.ExpiresAt).HasColumnName("ExpiresAt").IsRequired();
        builder.Property(r => r.RevokedAt).HasColumnName("RevokedAt");
        builder.Property(r => r.ReplacedByToken).HasColumnName("ReplacedByToken").HasMaxLength(256);
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(r => r.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(r => r.Token).IsUnique();
        builder.HasIndex(r => r.UserId);

        builder.HasQueryFilter(r => !r.DeletedDate.HasValue);
    }
}
