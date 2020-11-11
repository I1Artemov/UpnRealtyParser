using Microsoft.EntityFrameworkCore;
using UpnRealtyParser.Business.Models;

namespace UpnRealtyParser.Business.Contexts
{
    public class RealtyParserContext : DbContext
    {
        public DbSet<PageLink> PageLinks { get; set; }
        public DbSet<UpnFlat> UpnFlats { get; set; }
        public DbSet<UpnRentFlat> UpnRentFlats { get; set; }
        public DbSet<UpnHouseInfo> UpnHouseInfos { get; set; }
        public DbSet<UpnAgency> UpnAgencies { get; set; }
        public DbSet<UpnFlatPhoto> UpnFlatPhotos{ get; set; }
        public DbSet<ParsingState> ParsingStates { get; set; }
        public DbSet<WebProxyInfo> WebProxyInfos { get; set; }
        public DbSet<SubwayStation> SubwayStations { get; set; }
        public DbSet<N1Flat> N1Flats { get; set; }
        public DbSet<N1HouseInfo> N1HouseInfos { get; set; }
        public DbSet<N1Agency> N1Agencies { get; set; }

        public RealtyParserContext(DbContextOptions<RealtyParserContext> options) : base(options) { }

        public RealtyParserContext(){ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // todo: Вынести в конфиг
            optionsBuilder.UseSqlServer(@"Server=.\MSSQL14LOCAL;Database=RealtyParser;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();
        }
    }
}
