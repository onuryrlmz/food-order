using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OptionTemplateValueOptionValueConfiguration : IEntityTypeConfiguration<OptionTemplateValueOptionValue>
{
    public void Configure(EntityTypeBuilder<OptionTemplateValueOptionValue> builder)
    {
        builder.ToTable("OptionTemplateValueOptionValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();
        builder.Property(b => b.OptionTemplateValueOptionId).HasColumnName("OptionTemplateValueOptionId").IsRequired();
        builder.Property(b => b.ProductId).HasColumnName("ProductId").IsRequired();
        builder.Property(b => b.Price).HasColumnName("Price").IsRequired();
        builder.Property(b => b.OrderIndex).HasColumnName("OrderIndex").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.OptionTemplateValueOption).WithMany(x => x.OptionTemplateValueOptionValues).HasForeignKey(x => x.OptionTemplateValueOptionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OptionTemplateValueOptionId);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}
