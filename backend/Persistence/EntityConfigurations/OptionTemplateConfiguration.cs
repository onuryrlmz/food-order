using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OptionTemplateConfiguration : IEntityTypeConfiguration<OptionTemplate>
{
    public void Configure(EntityTypeBuilder<OptionTemplate> builder)
    {
        builder.ToTable("OptionTemplate").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();
        builder.Property(b => b.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(b => b.Name).HasColumnName("Name").IsRequired();
        builder.Property(b => b.Description).HasColumnName("Description");
        builder.Property(b => b.MinCount).HasColumnName("MinCount").IsRequired();
        builder.Property(b => b.MaxCount).HasColumnName("MaxCount").IsRequired();
        builder.Property(b => b.OrderIndex).HasColumnName("OrderIndex").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.Restaurant).WithMany().HasForeignKey(x => x.RestaurantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(b => b.OptionTemplateValues).WithOne(x => x.OptionTemplate).HasForeignKey(x => x.OptionTemplateId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RestaurantId);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}