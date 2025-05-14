using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MenuOptionValueOptionConfiguration : IEntityTypeConfiguration<MenuOptionValueOption>
{
    public void Configure(EntityTypeBuilder<MenuOptionValueOption> builder)
    {
        builder.ToTable("MenuOptionValueOption").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.MenuOptionValue);
        builder.HasMany(b => b.MenuOptionValueOptionValues);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}