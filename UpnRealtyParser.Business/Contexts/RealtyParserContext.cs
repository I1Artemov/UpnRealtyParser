using Microsoft.EntityFrameworkCore;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Contexts
{
    public class RealtyParserContext : DbContext
    {
        public DbSet<PageLink> PageLinks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\MSSQL14LOCAL;Database=RealtyParser;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();
        }
    }
}
