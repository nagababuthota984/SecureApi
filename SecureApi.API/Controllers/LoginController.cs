using Microsoft.AspNetCore.Mvc;
using SecureApi.Api.Identity.Utilities;

namespace SecureApi.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login(string username, string password)
        {
            return Ok(IdentityHelper.GenerateToken());
        }
        [HttpPost("register")]
        public IActionResult Register(string username, string password, string email)
        {
            return BadRequest();
        }
        
    }
}
