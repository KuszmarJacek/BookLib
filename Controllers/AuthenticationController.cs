using BookLib.ActionFilters;
using BookLib.DTOs;
using BookLib.Services;
using Microsoft.AspNetCore.Mvc; 

namespace BookLib.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDTO userForRegistrationDTO)
        {
            var result = await _service.AuthenticationService.RegisterUser(userForRegistrationDTO);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDTO userForAuthenticationDTO)
        {
            if (!await _service.AuthenticationService.ValidateUser(userForAuthenticationDTO))
            {
                return Unauthorized();
            }

            var tokenDTO = await _service.AuthenticationService.CreateToken(extendRefreshToken: true);

            return Ok(tokenDTO);
        }
    }
}
