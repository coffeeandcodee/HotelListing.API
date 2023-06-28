using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        //We need a different DTO for login, as userDto would request First and Last Names.
        //All we really need is email and password 
        Task<AuthResponseDto> Login(LoginDto loginDto);

        //In the instance of a request coming in and a token expired, the request fails.
        //Refresh tokens are passed in this case. This stops user from having to login repeatedly 
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);


    }
}
