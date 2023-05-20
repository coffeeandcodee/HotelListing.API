using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;
        //Local "context" field needed for the CountriesRepository specific methods
        //Previously, base(context) was enough as all methods were inherited 
        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(q => q.Hotels)
               .FirstOrDefaultAsync(q => q.Id == id);
            //FirstOrDefault returns null if value not found
        }
    }
}
