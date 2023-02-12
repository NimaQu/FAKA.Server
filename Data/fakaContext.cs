using FAKA.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Data;

public class FakaContext : IdentityDbContext<IdentityUser>
{
    public FakaContext(DbContextOptions<FakaContext> options) : base(options)
    {
    }

    public DbSet<Product> Product { get; set; } = default!;

    public DbSet<Key> Key { get; set; } = default!;

    public DbSet<Order> Order { get; set; } = default!;

    public DbSet<ProductGroup> ProductGroup { get; set; } = default!;

    public DbSet<Transaction> Transaction { get; set; } = default!;

    public DbSet<Gateway> Gateway { get; set; } = default!;

    public DbSet<Announcement> Announcements { get; set; } = default!;
    
    public DbSet<AssignedKey> AssignedKey { get; set; } = default!;

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added) ((BaseEntity)entity.Entity).CreatedAt = now;
            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}