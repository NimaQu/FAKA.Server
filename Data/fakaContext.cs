using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using faka.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
    }
}
