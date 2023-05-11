using Fantasy_Web_API.Data;
using Fantasy_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Shared_Classes.Models;
using System.Text;

namespace Fantasy_Web_API.Controllers
{
    [Route("user/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration _config;

        public LoginController(ApplicationDbContext context, IConfiguration config)
        {
            _db = context;
            _config = config;
        }

        // Handle user login and return a JWT token if authentication is successful.
        [AllowAnonymous] 
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] UserLogin userLogin) // note: make a dto
        {
            var user = await Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        // Authenticate the user and return their user object if they exist
        private async Task<UserModel> Authenticate(UserLogin userLogin)
        {
            // generate and return user object if user exists
            var currentUser = await _db.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == userLogin.Username.ToLower() && x.PasswordHash == userLogin.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }

        // Generate a JWT for the provided user object
        private string Generate(UserModel user)
        {
            // Set up security key for JWT signing
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            // Set up signing credentials for the JWT
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define the claims to be included in the JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create a JWT containing the specified claims and signing credentials

            var token = new JwtSecurityToken(_config["JwtSettings:Issuer"],
              _config["JwtSettings:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            // Generate a JWT string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
