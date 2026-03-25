using Domain.Entities.Buyer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Review").HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("Id").IsRequired();
        builder.Property(r => r.OrderId).HasColumnName("OrderId").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(r => r.RestaurantId).HasColumnName("RestaurantId").IsRequired();
        builder.Property(r => r.Rating).HasColumnName("Rating").IsRequired();
        builder.Property(r => r.Comment).HasColumnName("Comment").HasMaxLength(1000);
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(r => r.DeletedDate).HasColumnName("DeletedDate");

        // One review per order
        builder.HasIndex(r => r.OrderId).IsUnique();
        // Index for restaurant reviews lookup
        builder.HasIndex(r => r.RestaurantId);

        builder.HasQueryFilter(r => !r.DeletedDate.HasValue);
    }
}