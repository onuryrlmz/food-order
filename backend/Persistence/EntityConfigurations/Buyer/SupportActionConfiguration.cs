using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class SupportActionConfiguration : IEntityTypeConfiguration<SupportAction>
{
    public void Configure(EntityTypeBuilder<SupportAction> builder)
    {
        builder.ToTable("SupportAction").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.TicketId).HasColumnName("TicketId").IsRequired();
        builder.Property(e => e.ActionType).HasColumnName("ActionType").IsRequired();
        builder.Property(e => e.ActionData).HasColumnName("ActionData").IsRequired();
        builder.Property(e => e.IsApproved).HasColumnName("IsApproved").IsRequired();
        builder.Property(e => e.IsExecuted).HasColumnName("IsExecuted").IsRequired();
        builder.Property(e => e.ApprovedByUserId).HasColumnName("ApprovedByUserId");
        builder.Property(e => e.ExecutedAt).HasColumnName("ExecutedAt");
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.Ticket).WithMany(t => t.Actions).HasForeignKey(e => e.TicketId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TicketId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
