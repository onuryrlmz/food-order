using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class OrderItemValueOptionConfiguration : IEntityTypeConfiguration<OrderItemValueOption>
{
    public void Configure(EntityTypeBuilder<OrderItemValueOption> builder)
    {
        builder.ToTable("OrderItemValueOption").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderItemValueId).HasColumnName("OrderItemValueId").IsRequired();
        builder.Property(e => e.MenuOptionValueOptionId).HasColumnName("MenuOptionValueOptionId").IsRequired();
        builder.Property(e => e.MenuOptionValueOptionValueId).HasColumnName("MenuOptionValueOptionValueId").IsRequired();
        builder.Property(e => e.ProductId).HasColumnName("ProductId").IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("Quantity").IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.TotalPrice).HasColumnName("TotalPrice").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.OrderItemValue).WithMany(v => v.OrderItemValueOptions).HasForeignKey(e => e.OrderItemValueId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MenuOptionValueOption).WithMany().HasForeignKey(e => e.MenuOptionValueOptionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.MenuOptionValueOptionValue).WithMany().HasForeignKey(e => e.MenuOptionValueOptionValueId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrderItemValueId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
