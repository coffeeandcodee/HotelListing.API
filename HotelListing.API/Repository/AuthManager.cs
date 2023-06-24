using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

//CHALLENGE 
//1. Create Endpoint Admins can use to create other admin users
//2. Secure Hotels controller as you see fit 
namespace HotelListing.API.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private ApiUser _user;

        //To avoid repitition in Refresh methods
        private const string _loginProvier = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        //Injecting configuration manager into AuthManager
        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
            this._logger = logger;
        }

        //To create a refresh token means storing in the database that a token has been issued to the user
        public async Task<string> CreateRefreshToken()
        {
            //When refreshing token, remove old one
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvier, _refreshToken);
            //Generate new token 
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvier, _refreshToken);
            //Create result. Generates then sets token in Database
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvier, _refreshToken, newRefreshToken);

            return newRefreshToken;
        }

        //Login operations for APIs validate whether user exists in the system 
        //This is different to web Apps, where cookies are created and a session is maintained for the user
        //Recall: APIS are STATELESS
        //Find user with the provided email

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            _logger.LogInformation($"Looking for user with email {loginDto}");
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (_user == null || isValidUser == false)
            {
                _logger.LogWarning($"User with email {loginDto.Email} was not found");
                return null;
            }

            //async methods must be awaited
            var token = await GenerateToken();
            _logger.LogInformation($"Token generated for User with email {loginDto.Email} | Token: {token}");

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };


            /*
            var user = await _userManager.FindByEmailAsync(loginDto.EmailAddress);
            if (user is null)
            {
                return default;
            }

            bool isValidCredentials = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isValidCredentials)
            {
                return default;
            }
            */


        }

        //Register method returns message if failure
        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email; //Assign the username the value of email

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                //If succesfully created, user added to the predefined User role
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;

        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecuirityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecuirityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null || _user.Id != request.UserId)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvier, _refreshToken, request.RefreshToken);

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };

            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }
        #region Challenge: Creating Admins
        /*
        public async Task<IEnumerable<IdentityError>> RegisterAdmin(ApiUserDto userDto)
        {
            var user = _mapper.Map<ApiUser>(userDto);
            user.UserName = userDto.Email; //Assign the username the value of email

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                //If succesfully created, user added to the predefined User role
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Administrator");
            }

            return result.Errors;

        }
        */
        #endregion

        //Implementing JWT authentication Pt. 2
        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Checking the roles of user passed into method
            var roles = await _userManager.GetRolesAsync(_user);

            //Does an operation against the list of roles
            //Generate list of Claims, where we state the type of claim 
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            //You can add claims to a user after registering them 
            var userClaims = await _userManager.GetClaimsAsync(_user);


            //Generating actual list of claims for JWT
            //Claims are bits of information about the user
            var claims = new List<Claim>
            {
                //Sub for JWTs represents the subject for which the token has been issued
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
            }
            .Union(userClaims).Union(roleClaims);
            //After lines 104 - 114, we have a list of Claims
            //This was all generating info about user. We then need to create the token

            //Token Object
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            //returning string that represents our token
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
