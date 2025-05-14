using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class MenuOptionValueOptionValueConfiguration : IEntityTypeConfiguration<MenuOptionValueOptionValue>
{
    public void Configure(EntityTypeBuilder<MenuOptionValueOptionValue> builder)
    {
        builder.ToTable("MenuOptionValueOptionValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(x => x.MenuOptionValueOption);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}