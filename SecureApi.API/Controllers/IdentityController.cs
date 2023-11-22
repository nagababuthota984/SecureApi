using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using SecureApi.API.ApiModels;
using SecureApi.API.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        public IdentityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto is null)
                return BadRequest("Invalid login details");
            else
            {
                AppUser user = await _userManager.FindByNameAsync(loginDto.Username);
                if (user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
                    var token = new JwtSecurityToken(
                        issuer: _config["JwtSettings:Issuer"],
                        audience: _config["JwtSettings:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                        );
                    await _signInManager.SignInAsync(user, true);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByNameAsync(registerDto.Username);
            if (userExists is not null)
                return BadRequest("Resource already exists.");
            AppUser user = new()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDto.Username
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest("Error creating the resource");
        }

        [HttpPost("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByNameAsync(registerDto.Username);
            if (userExists != null)
                return BadRequest("admin already exists");

            AppUser user = new()
            {
                Email = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerDto.Username
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest("Error creating admin user");

            if (!await _roleManager.RoleExistsAsync(nameof(AppUserRoles.Admin)))
                await _roleManager.CreateAsync(new IdentityRole(nameof(AppUserRoles.Admin)));
            if (!await _roleManager.RoleExistsAsync(nameof(AppUserRoles.User)))
                await _roleManager.CreateAsync(new IdentityRole(nameof(AppUserRoles.User)));

            if (await _roleManager.RoleExistsAsync(nameof(AppUserRoles.Admin)))
            {
                await _userManager.AddToRoleAsync(user, nameof(AppUserRoles.Admin));
            }
            return Ok("Admin created successfully");
        }
    }
}
