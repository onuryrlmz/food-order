using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.TypeId).HasColumnName("TypeId").IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Message).HasColumnName("Message").HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Data).HasColumnName("Data").HasMaxLength(2000);
        builder.Property(e => e.IsRead).HasColumnName("IsRead").IsRequired();
        builder.Property(e => e.ReadAt).HasColumnName("ReadAt");
        builder.Property(e => e.RelatedOrderId).HasColumnName("RelatedOrderId");
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.RelatedOrderId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
