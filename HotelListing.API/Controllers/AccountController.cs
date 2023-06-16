using HotelListing.API.Contracts;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


//CHALLENGE 
//1. Create Endpoint Admins can use to create other admin users
//2. Secure Hotels controller as you see fit 
namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AccountController(IAuthManager authManager)
        {
            this._authManager = authManager;
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        //Recall how in earlier controller methods, the id argument was coming from the LINK 
        //[FromBody] used to specify parameter coming from BODY, not LINK
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            var errors = await _authManager.Register(apiUserDto);

            //If there are errors, we want to iterate through them and add to model state
           
            if (errors.Any())
            {
                foreach(var error in errors)
                {
                    //Error object comes with a code and a message 
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();

        }

        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        //Recall how in earlier controller methods, the id argument was coming from the LINK 
        //[FromBody] used to specify parameter coming from BODY, not LINK
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authManager.Login(loginDto);

            if (authResponse == null)
            {
                //Use 
                return Unauthorized();
            }

            return Ok(authResponse);
           
        }

        // POST: api/Account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        //Recall how in earlier controller methods, the id argument was coming from the LINK 
        //[FromBody] used to specify parameter coming from BODY, not LINK
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            if (authResponse == null)
            {
                //Use 
                return Unauthorized();
            }

            return Ok(authResponse);

        }

    }
}
