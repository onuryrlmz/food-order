using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SellerDetailConfiguration : IEntityTypeConfiguration<SellerDetail>
{
    public void Configure(EntityTypeBuilder<SellerDetail> builder)
    {
        builder.ToTable("SellerDetail").HasKey(c => c.Id);
    }
}