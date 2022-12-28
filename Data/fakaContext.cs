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

        public DbSet<faka.Models.Product> Product { get; set; } = default!;

        public DbSet<faka.Models.Key> Key { get; set; } = default!;

        public DbSet<faka.Models.Order> Bought { get; set; } = default!;
    }
}
