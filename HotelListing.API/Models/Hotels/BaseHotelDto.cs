using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotels
{
    public abstract class BaseHotelDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        //Making property nullable means it accepts nothing
        //As opposed to defaulting the rating to 0
        public double? Rating { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CountryId { get; set; }
    }
}
