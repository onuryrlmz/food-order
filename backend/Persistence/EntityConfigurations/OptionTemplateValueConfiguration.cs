using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OptionTemplateValueConfiguration : IEntityTypeConfiguration<OptionTemplateValue>
{
    public void Configure(EntityTypeBuilder<OptionTemplateValue> builder)
    {
        builder.ToTable("OptionTemplateValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();
        builder.Property(b => b.OptionTemplateId).HasColumnName("OptionTemplateId").IsRequired();
        builder.Property(b => b.ProductId).HasColumnName("ProductId").IsRequired();
        builder.Property(b => b.Price).HasColumnName("Price").IsRequired();
        builder.Property(b => b.OrderIndex).HasColumnName("OrderIndex").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.OptionTemplate).WithMany(x => x.OptionTemplateValues).HasForeignKey(x => x.OptionTemplateId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.OptionTemplateValueOptions).WithOne(x => x.OptionTemplateValue).HasForeignKey(x => x.OptionTemplateValueId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OptionTemplateId);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}