using Fantasy_Web_API.Data;
using Fantasy_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fantasy_Web_API.Controllers
{
    [Route("user/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration _config;

        public LoginController(ApplicationDbContext context)
        {
            _db = context;
        }

        // Handle user login and return a JWT token if authentication is successful.
        [AllowAnonymous] 
        [HttpPost]
        public ActionResult Login([FromBody] UserLogin userLogin) // note: make a dto
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        // Authenticate the user and return their user object if they exist
        private UserModel Authenticate(UserLogin userLogin)
        {
            // generate and return user object if user exists
            var currentUser = UserConstants.Users.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);
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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            // Set up signing credentials for the JWT
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define the claims to be included in the JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create a JWT containing the specified claims and signing credentials

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            // Generate a JWT string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
