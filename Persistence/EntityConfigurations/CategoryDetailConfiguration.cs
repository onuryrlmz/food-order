using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class CategoryDetailConfiguration : IEntityTypeConfiguration<CategoryDetail>
{
    public void Configure(EntityTypeBuilder<CategoryDetail> builder)
    {
        builder.ToTable("CategoryDetail").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(b => b.Category);

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}