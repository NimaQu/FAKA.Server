using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using faka.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace faka.Data
{
    public class fakaContext : IdentityDbContext<IdentityUser>
    {
        public fakaContext (DbContextOptions<fakaContext> options): base(options)
        {
        }

        public DbSet<Product> Product { get; set; } = default!;

        public DbSet<Key> Key { get; set; } = default!;

        public DbSet<Order> Order { get; set; } = default!;
        
        public DbSet<ProductGroup> ProductGroup { get; set; } = default!;
        
        public DbSet<Transaction> Transaction { get; set; } = default!;
        
        public DbSet<Gateway> Gateway { get; set; } = default!;
        
        public DbSet<Announcement> Announcements { get; set; } = default!;

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

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
