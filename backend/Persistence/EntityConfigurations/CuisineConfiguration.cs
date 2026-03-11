using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.ToTable("Cuisine").HasKey(c => c.Id);

        builder.Property(b => b.Id).HasColumnName("Id").IsRequired();

        builder.Property(b => b.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(b => b.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(b => b.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(b => !b.DeletedDate.HasValue);
    }
}