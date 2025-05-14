using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MenuOptionValueConfiguration : IEntityTypeConfiguration<MenuOptionValue>
{
    public void Configure(EntityTypeBuilder<MenuOptionValue> builder)
    {
        builder.ToTable("MenuOptionValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.MenuOption);
        builder.HasMany(b => b.MenuOptionValueOptions);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}