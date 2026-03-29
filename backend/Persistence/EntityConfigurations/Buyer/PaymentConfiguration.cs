using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.SellerId).HasColumnName("SellerId").IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.SellerPayoutAmount).HasColumnName("SellerPayoutAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.CommissionAmount).HasColumnName("CommissionAmount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(e => e.ProviderPaymentId).HasColumnName("ProviderPaymentId").HasMaxLength(200);
        builder.Property(e => e.ProviderConversationId).HasColumnName("ProviderConversationId").HasMaxLength(200);
        builder.Property(e => e.ProviderTransactionId).HasColumnName("ProviderTransactionId").HasMaxLength(200);
        builder.Property(e => e.ProviderFraudStatus).HasColumnName("ProviderFraudStatus").HasMaxLength(50);
        builder.Property(e => e.PaymentOptionId).HasColumnName("PaymentOptionId").IsRequired();
        builder.Property(e => e.CardLastFourDigits).HasColumnName("CardLastFourDigits").HasMaxLength(4);
        builder.Property(e => e.CardType).HasColumnName("CardType").HasMaxLength(50);
        builder.Property(e => e.CardAssociation).HasColumnName("CardAssociation").HasMaxLength(50);
        builder.Property(e => e.CardAlias).HasColumnName("CardAlias").HasMaxLength(100);
        builder.Property(e => e.ErrorMessage).HasColumnName("ErrorMessage").HasMaxLength(1000);
        builder.Property(e => e.ErrorCode).HasColumnName("ErrorCode").HasMaxLength(50);
        builder.Property(e => e.CompletedAt).HasColumnName("CompletedAt");
        builder.Property(e => e.FailedAt).HasColumnName("FailedAt");
        builder.Property(e => e.RefundedAt).HasColumnName("RefundedAt");
        builder.Property(e => e.RefundTransactionId).HasColumnName("RefundTransactionId").HasMaxLength(200);
        builder.Property(e => e.RefundReason).HasColumnName("RefundReason").HasMaxLength(500);
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Order).WithMany().HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.SellerId);
        builder.HasIndex(e => e.StatusId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
