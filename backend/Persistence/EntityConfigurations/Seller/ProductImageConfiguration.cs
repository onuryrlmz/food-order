using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Seller;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImage").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.ProductId).HasColumnName("ProductId");
        builder.Property(e => e.Url).HasColumnName("Url").HasMaxLength(500).IsRequired();
        builder.Property(e => e.OrderIndex).HasColumnName("OrderIndex").IsRequired();
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(e => e.ProductId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
