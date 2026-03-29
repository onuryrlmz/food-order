using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.ToTable("SupportMessage").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.TicketId).HasColumnName("TicketId").IsRequired();
        builder.Property(e => e.SenderType).HasColumnName("SenderType").IsRequired();
        builder.Property(e => e.Content).HasColumnName("Content").IsRequired();
        builder.Property(e => e.AiModelUsed).HasColumnName("AiModelUsed").HasMaxLength(100);
        builder.Property(e => e.TokensUsed).HasColumnName("TokensUsed");
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Ticket).WithMany(t => t.Messages).HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TicketId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
