using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations.Buyer;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTicket").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId");
        builder.Property(e => e.RestaurantId).HasColumnName("RestaurantId");
        builder.Property(e => e.TopicId).HasColumnName("TopicId").IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("StatusId").IsRequired();
        builder.Property(e => e.Subject).HasColumnName("Subject").HasMaxLength(300).IsRequired();
        builder.Property(e => e.IsEscalated).HasColumnName("IsEscalated").IsRequired();
        builder.Property(e => e.Rating).HasColumnName("Rating");
        builder.Property(e => e.RatingComment).HasColumnName("RatingComment").HasMaxLength(1000);
        builder.Property(e => e.ResolvedAt).HasColumnName("ResolvedAt");
        builder.Property(e => e.ClosedAt).HasColumnName("ClosedAt");
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Messages).WithOne(m => m.Ticket).HasForeignKey(m => m.TicketId);
        builder.HasMany(e => e.Actions).WithOne(a => a.Ticket).HasForeignKey(a => a.TicketId);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.OrderId);
        builder.HasIndex(e => e.StatusId);

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}
