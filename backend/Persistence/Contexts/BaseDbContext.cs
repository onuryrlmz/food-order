using System.Reflection;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Entities.Courier;
using Domain.Entities.Seller;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Seeds;

namespace Persistence.Contexts;

public sealed class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
    {
        Configuration = configuration;
        Database?.Migrate();
    }

    private IConfiguration Configuration { get; set; }

    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CouponMenu> CouponMenus { get; set; }
    public DbSet<CouponCategory> CouponCategories { get; set; }
    public DbSet<UserCoupon> UserCoupons { get; set; }
    public DbSet<UserExternalInfo> UserExternalInfos { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItemValue> OrderItemValues { get; set; }
    public DbSet<OrderItemValueOption> OrderItemValueOptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    // Buyer
    public DbSet<Review> Reviews { get; set; }
    public DbSet<FavoriteRestaurant> FavoriteRestaurants { get; set; }

    // Auth
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    // Seller - Subscription
    public DbSet<SubscriptionUsage> SubscriptionUsages { get; set; }

    // Courier
    public DbSet<CourierCompany> CourierCompanies { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<RestaurantCourierAgreement> RestaurantCourierAgreements { get; set; }
    public DbSet<DeliveryAssignment> DeliveryAssignments { get; set; }
    public DbSet<CourierEarning> CourierEarnings { get; set; }
    public DbSet<CourierLocationHistory> CourierLocationHistories { get; set; }

    // Buyer - Tip & Search
    public DbSet<Tip> Tips { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }

    // Buyer - Notification
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }

    // Buyer - Scheduled Order
    public DbSet<ScheduledOrder> ScheduledOrders { get; set; }

    // Buyer - AI Support
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<SupportMessage> SupportMessages { get; set; }
    public DbSet<SupportAction> SupportActions { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = ChangeTracker.Entries<Entity<Guid>>().Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
            _ = entry.State switch
            {
                EntityState.Added => entry.Entity.CreatedDate = DateTime.UtcNow,
                EntityState.Modified => entry.Entity.UpdatedDate = DateTime.UtcNow
            };
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new FakeData().Seed(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}