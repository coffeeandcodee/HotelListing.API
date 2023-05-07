using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

//Database context can be thought of to be a contract between app and database
public class HotelListingDbContext : DbContext
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
        modelBuilder.Entity<Country>().HasData(
            new Country
            {
                Id = 1,
                Name = "Jamaica",
                ShortName = "JM"
            },
            new Country
            {
                Id = 2,
                Name = "Bahamas",
                ShortName = "BH"
            },
            new Country
            {
                Id = 3,
                Name = "Cayman Island",
                ShortName = "CI"
            }

        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel
            {
                Id = 1,
                Name = "Sandals Resort and Spa",
                Address = "Negril",
                CountryId = 1,
                Rating = 4.5
            },
            new Hotel
            {
                Id = 2,
                Name = "Comfort Suites",
                Address = "George Town",
                CountryId = 3,
                Rating = 4.3
            },
            new Hotel
            {
                Id = 3,
                Name = "Grand Palldium",
                Address = "Nassua",
                CountryId = 2,
                Rating = 4
            }


        );
    }

}
