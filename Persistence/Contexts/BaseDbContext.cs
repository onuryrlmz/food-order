using System.Reflection;
using Domain.Entities.Common;
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

        //Database?.EnsureDeleted();
        Database?.EnsureCreated();
        Database?.Migrate();
    }

    private IConfiguration Configuration { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<SellerDetail> SellerDetails { get; set; }

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