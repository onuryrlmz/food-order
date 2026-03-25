using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem").HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id).HasColumnName("Id").IsRequired();
        builder.Property(oi => oi.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(oi => oi.MenuId).HasColumnName("MenuId").IsRequired();
        builder.Property(oi => oi.Quantity).HasColumnName("Quantity").IsRequired();
        builder.Property(oi => oi.UnitPrice).HasColumnName("UnitPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(oi => oi.TotalPrice).HasColumnName("TotalPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(oi => oi.ItemSnapshotJson).HasColumnName("ItemSnapshotJson").HasColumnType("LONGTEXT");
        builder.Property(oi => oi.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oi => oi.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(oi => oi.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(oi => !oi.DeletedDate.HasValue);
    }
}