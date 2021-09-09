using Microsoft.EntityFrameworkCore;
using UpnRealtyParser.Business.Models;
using UpnRealtyParser.Business.Models.N1;

namespace UpnRealtyParser.Business.Contexts
{
    public class RealtyParserContext : DbContext
    {
        public DbSet<PageLink> PageLinks { get; set; }
        public DbSet<UpnFlat> UpnFlats { get; set; }
        public DbSet<UpnFlatVmForTable> UpnFlatVmForTables { get; set; }
        public DbSet<N1FlatVmForTable> N1FlatVmForTables { get; set; }
        public DbSet<N1RentFlatVmForTable> N1RentFlatVmForTables { get; set; }
        public DbSet<HouseSitelessVM> HouseSitelessVms { get; set; }
        public DbSet<AgencySitelessVM> AgencySitelessVms { get; set; }
        public DbSet<UpnRentFlat> UpnRentFlats { get; set; }
        public DbSet<UpnRentFlatVmForTable> UpnRentFlatVmForTables { get; set; }
        public DbSet<UpnHouseInfo> UpnHouseInfos { get; set; }
        public DbSet<UpnAgency> UpnAgencies { get; set; }
        public DbSet<UpnFlatPhoto> UpnFlatPhotos{ get; set; }
        public DbSet<ParsingState> ParsingStates { get; set; }
        public DbSet<WebProxyInfo> WebProxyInfos { get; set; }
        public DbSet<SubwayStation> SubwayStations { get; set; }
        public DbSet<N1Flat> N1Flats { get; set; }
        public DbSet<N1RentFlat> N1RentFlats { get; set; }
        public DbSet<N1HouseInfo> N1HouseInfos { get; set; }
        public DbSet<N1Agency> N1Agencies { get; set; }
        public DbSet<N1FlatPhoto> N1FlatPhotos { get; set; }
        public DbSet<SimilarHouse> SimilarHouses { get; set; }
        public DbSet<AveragePriceStat> AveragePriceStats { get; set; }
        public DbSet<ServiceStage> ServiceStages { get; set; }
        public DbSet<PaybackPeriodPoint> PaybackPeriodPoints { get; set; }
        public DbSet<ApartmentPayback> ApartmentPaybacks { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        public RealtyParserContext(DbContextOptions<RealtyParserContext> options) : base(options) { }

        public RealtyParserContext(){ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // todo: Вынести в конфиг
            optionsBuilder.UseSqlServer(@"Data Source=.\MSSQL14LOCAL;Database=RealtyParser;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.Entity<UpnFlatVmForTable>(entity => {entity.ToTable("vUpnFlatAdditional");});
            modelBuilder.Entity<UpnRentFlatVmForTable>(entity => { entity.ToTable("vUpnRentFlatAdditional"); });
            modelBuilder.Entity<N1FlatVmForTable>(entity => { entity.ToTable("vN1FlatAdditional"); });
            modelBuilder.Entity<N1RentFlatVmForTable>(entity => { entity.ToTable("vN1RentFlatAdditional"); });
            modelBuilder.Entity<HouseSitelessVM>(entity => { entity.ToTable("vHousesUnitedInfo"); });
            modelBuilder.Entity<AgencySitelessVM>(entity => { entity.ToTable("vAgenciesUnitedInfo"); });
        }
    }
}
