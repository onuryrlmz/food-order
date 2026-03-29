using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistory").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(e => e.Note).HasColumnName("Note").HasMaxLength(500);
        builder.Property(e => e.OccurredAt).HasColumnName("OccurredAt").IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Order).WithMany().HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrderId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
