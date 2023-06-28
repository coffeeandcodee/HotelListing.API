using HotelListing.API.Data.Configurations;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

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





}
