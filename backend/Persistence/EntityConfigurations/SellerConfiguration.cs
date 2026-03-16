using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SellerConfiguration : IEntityTypeConfiguration<Domain.Entities.Seller.Seller>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Seller.Seller> builder)
    {
        builder.ToTable("Seller").HasKey(c => c.Id);
    }
}