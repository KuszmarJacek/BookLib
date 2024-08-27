using BookLib.ActionFilters;
using BookLib.DTOs;
using BookLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookLib.Controllers
{
    [Route("/api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IServiceManager _service;

        public TokenController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDTO)
        {
            var result = await _service.AuthenticationService.RefreshToken(tokenDTO);
            return Ok(result);
        }
    }
}
