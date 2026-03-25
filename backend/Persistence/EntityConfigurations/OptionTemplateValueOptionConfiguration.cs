using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OptionTemplateValueOptionConfiguration : IEntityTypeConfiguration<OptionTemplateValueOption>
{
    public void Configure(EntityTypeBuilder<OptionTemplateValueOption> builder)
    {
        builder.ToTable("OptionTemplateValueOption").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();
        builder.Property(b => b.OptionTemplateValueId).HasColumnName("OptionTemplateValueId").IsRequired();
        builder.Property(b => b.Name).HasColumnName("Name").IsRequired();
        builder.Property(b => b.Description).HasColumnName("Description");
        builder.Property(b => b.MinCount).HasColumnName("MinCount").IsRequired();
        builder.Property(b => b.MaxCount).HasColumnName("MaxCount").IsRequired();
        builder.Property(b => b.OrderIndex).HasColumnName("OrderIndex").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.OptionTemplateValue).WithMany(x => x.OptionTemplateValueOptions).HasForeignKey(x => x.OptionTemplateValueId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.OptionTemplateValueOptionValues).WithOne(x => x.OptionTemplateValueOption).HasForeignKey(x => x.OptionTemplateValueOptionId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OptionTemplateValueId);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}