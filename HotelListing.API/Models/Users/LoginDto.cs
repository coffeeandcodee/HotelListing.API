
using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Users
{
    public class LoginDto 
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        //Entity core has its own default validations for password
        //i.e password already has a minimum length 
        [StringLength(15, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 6)]
        public string Password { get; set; }


    }




}
