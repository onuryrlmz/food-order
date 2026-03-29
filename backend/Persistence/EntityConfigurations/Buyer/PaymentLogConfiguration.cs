using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class PaymentLogConfiguration : IEntityTypeConfiguration<PaymentLog>
{
    public void Configure(EntityTypeBuilder<PaymentLog> builder)
    {
        builder.ToTable("PaymentLog").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId");
        builder.Property(e => e.PaymentId).HasColumnName("PaymentId");
        builder.Property(e => e.Action).HasColumnName("Action").HasMaxLength(100).IsRequired();
        builder.Property(e => e.RequestData).HasColumnName("RequestData");
        builder.Property(e => e.ResponseData).HasColumnName("ResponseData");
        builder.Property(e => e.StatusCode).HasColumnName("StatusCode");
        builder.Property(e => e.IsSuccess).HasColumnName("IsSuccess").IsRequired();
        builder.Property(e => e.ErrorMessage).HasColumnName("ErrorMessage").HasMaxLength(1000);
        builder.Property(e => e.DurationMs).HasColumnName("DurationMs");
        builder.Property(e => e.IpAddress).HasColumnName("IpAddress").HasMaxLength(50);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.PaymentId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
