using Book.Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Library.Data
{
    public class ApplicationDbContext : DbContext
    {
        public IConfigurationRoot Configuration { get; set; }
        public static string ConnectionString { get; private set; }
        private readonly DbContextOptions<ApplicationDbContext> _connInfo = null;

        public ApplicationDbContext(string connInfo)
        {
            _connInfo = GetConnection(connInfo).Options;
        }

        public DbContextOptionsBuilder<ApplicationDbContext> GetConnection(string conn)
        {
            DbContextOptionsBuilder<ApplicationDbContext> connBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            connBuilder.UseSqlServer(conn);
            return connBuilder;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=.;Initial Catalog=BookLibDb; Integrated Security = True; trusted_connection=true;encrypt=false;");
        }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder) {}

        public DbSet<BookEntity>? Books { get; set; }
    }
}
