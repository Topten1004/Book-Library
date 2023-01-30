using Book.Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Library.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder) {}

        public DbSet<BookEntity>? Books { get; set; }
    }
}
