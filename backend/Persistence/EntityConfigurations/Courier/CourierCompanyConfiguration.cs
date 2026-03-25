using Domain.Entities.Courier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Courier;

public class CourierCompanyConfiguration : IEntityTypeConfiguration<CourierCompany>
{
    public void Configure(EntityTypeBuilder<CourierCompany> builder)
    {
        builder.ToTable("CourierCompany").HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("Id").IsRequired();
        builder.Property(c => c.Name).HasColumnName("Name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.LegalName).HasColumnName("LegalName").HasMaxLength(300).IsRequired();
        builder.Property(c => c.TaxCode).HasColumnName("TaxCode").HasMaxLength(50).IsRequired();
        builder.Property(c => c.TaxArea).HasColumnName("TaxArea").HasMaxLength(100);
        builder.Property(c => c.IBAN).HasColumnName("IBAN").HasMaxLength(34).IsRequired();
        builder.Property(c => c.Phone).HasColumnName("Phone").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Email).HasColumnName("Email").HasMaxLength(200);
        builder.Property(c => c.ContactPerson).HasColumnName("ContactPerson").HasMaxLength(200);
        builder.Property(c => c.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(c => c.CompanyTypeId).HasColumnName("CompanyTypeId").IsRequired();
        builder.Property(c => c.IdentityNumber).HasColumnName("IdentityNumber").HasMaxLength(20);
        builder.Property(c => c.CommissionRate).HasColumnName("CommissionRate").HasPrecision(5, 2).IsRequired();
        builder.Property(c => c.LogoUrl).HasColumnName("LogoUrl").HasMaxLength(500);
        builder.Property(c => c.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(c => c.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(c => c.DeletedDate).HasColumnName("DeletedDate");

        builder.HasMany(c => c.Couriers).WithOne(c => c.CourierCompany).HasForeignKey(c => c.CourierCompanyId);
        builder.HasMany(c => c.Agreements).WithOne(a => a.CourierCompany).HasForeignKey(a => a.CourierCompanyId);

        builder.HasQueryFilter(c => !c.DeletedDate.HasValue);
    }
}
