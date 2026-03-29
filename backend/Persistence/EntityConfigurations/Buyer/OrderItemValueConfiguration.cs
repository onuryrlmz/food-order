using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class OrderItemValueConfiguration : IEntityTypeConfiguration<OrderItemValue>
{
    public void Configure(EntityTypeBuilder<OrderItemValue> builder)
    {
        builder.ToTable("OrderItemValue").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderItemId).HasColumnName("OrderItemId").IsRequired();
        builder.Property(e => e.MenuOptionId).HasColumnName("MenuOptionId").IsRequired();
        builder.Property(e => e.MenuOptionValueId).HasColumnName("MenuOptionValueId").IsRequired();
        builder.Property(e => e.ProductId).HasColumnName("ProductId").IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("Quantity").IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TotalPrice).HasColumnName("TotalPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.OrderItem).WithMany().HasForeignKey(e => e.OrderItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MenuOption).WithMany().HasForeignKey(e => e.MenuOptionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MenuOptionValue).WithMany().HasForeignKey(e => e.MenuOptionValueId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.OrderItemValueOptions).WithOne(o => o.OrderItemValue).HasForeignKey(o => o.OrderItemValueId);

        builder.HasIndex(e => e.OrderItemId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
