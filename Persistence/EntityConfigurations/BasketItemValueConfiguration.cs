using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class BasketItemValueConfiguration : IEntityTypeConfiguration<BasketItemValue>
{
    public void Configure(EntityTypeBuilder<BasketItemValue> builder)
    {
        builder.ToTable("BasketItemValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(b => b.BasketItem);
        builder.HasMany(b => b.BasketItemValueItemValues);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}