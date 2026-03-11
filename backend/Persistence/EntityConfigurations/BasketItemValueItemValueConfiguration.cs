using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class BasketItemValueItemValueConfiguration : IEntityTypeConfiguration<BasketItemValueItemValue>
{
    public void Configure(EntityTypeBuilder<BasketItemValueItemValue> builder)
    {
        builder.ToTable("BasketItemValueItemValue").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(b => b.BasketItemValue);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}