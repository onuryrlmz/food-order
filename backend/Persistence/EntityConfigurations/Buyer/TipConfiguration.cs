using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class TipConfiguration : IEntityTypeConfiguration<Tip>
{
    public void Configure(EntityTypeBuilder<Tip> builder)
    {
        builder.ToTable("Tip").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.CourierId).HasColumnName("CourierId");
        builder.Property(e => e.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.PresetPercentage).HasColumnName("PresetPercentage");
        builder.Property(e => e.IsPreDelivery).HasColumnName("IsPreDelivery").IsRequired();
        builder.Property(e => e.IsSettled).HasColumnName("IsSettled").IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Order).WithMany().HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CourierId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
