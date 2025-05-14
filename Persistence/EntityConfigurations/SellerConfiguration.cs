using Base.Enums;
using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("Sellers").HasKey(c => c.Id);

        Seller[] seeds =
        {
            new()
            {
                Id = Guid.Parse("bab60c66-11df-4c2d-8fc3-b8702664d9cf"),
                CreatedDate = DateTime.Now,
                CompanyType = (short)AuthorizationServiceEnums.CompanyTypeEnums.Company,
                CompanyStatus = (short)AuthorizationServiceEnums.CompanyStatusEnums.Pending,
                Name = "YRLMZ Teknoloji",
                LegalName = "YRLMZ Teknoloji Ltd. Şti.",
                TaxCode = "1234567890",
                TaxArea = "Nilüfer",
                IBAN = "TR260006266822193294982978",
                IsEInvoiceAvaible = true
            }
        };
        builder.HasData(seeds);
    }
}