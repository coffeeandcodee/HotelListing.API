namespace HotelListing.API.Models.Country;

//Auto-mapper looks for fields of the same name. Important to maintain the same naming conventions
public class GetCountriesDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
}

  
