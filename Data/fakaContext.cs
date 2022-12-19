using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using faka.Models;

namespace faka.Data
{
    public class fakaContext : DbContext
    {
        public fakaContext (DbContextOptions<fakaContext> options)
            : base(options)
        {
        }

        public DbSet<faka.Models.Product> Product { get; set; } = default!;
    }
}
