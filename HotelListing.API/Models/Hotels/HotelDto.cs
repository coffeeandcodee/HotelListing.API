using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.API.Models.Hotels;


public class HotelDto : BaseHotelDto
{
    public int Id { get; set; }

}