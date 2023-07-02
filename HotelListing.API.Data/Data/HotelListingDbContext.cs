using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;

namespace HotelListing.API.Data;

//Database context can be thought of to be a contract between app and database
public class HotelListingDbContext : IdentityDbContext<ApiUser>
{
    public HotelListingDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Makes sense to seed the country first as hotel has a dependency on country. Country needs to exist before hotel exists
        base.OnModelCreating(modelBuilder);
        //Line below added during "Add default user roles" video
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new HotelConfiguration());

    }

    public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDbContext>
    {
        public HotelListingDbContext CreateDbContext(string[] args)
        {
            //Fetching configuration
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HotelListingDbContext>();
            var conn = config.GetConnectionString("HotelListingDbConnectionString");
            optionsBuilder.UseSqlServer(conn);

            return new HotelListingDbContext(optionsBuilder.Options);
        }
    }
}
