using HotelListing.API.Models.Hotels;

namespace HotelListing.API.Models.Country;

public class GetCountryDto : BaseCountryDto
{

    public int Id { get; set; }
 

    //A DTO should never have a field that is directly related to our model type. 
    //DTOs should only communicate with DTOs.
    //The only time a DTO can be seen to be used next to data model is when we're doing a 
    //mapping operation
    public List<HotelDto> Hotels { get; set; }
}

  
