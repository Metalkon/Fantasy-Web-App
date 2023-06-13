using Fantasy_Web_API.Data;
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
using Fantasy_Web_API.Models;
using Fantasy_Web_API.Services;

namespace Fantasy_Web_API.Controllers
{
    [Route("user/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public LoginController(ApplicationDbContext context, IConfiguration config, IEmailSender emailSender)
        {
            _db = context;
            _config = config;
            this._emailSender = emailSender;
        }

        // Handle user login and return a JWT token if authentication is successful.
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid || userLogin == null || string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrEmpty(userLogin.Email))
            {
                return BadRequest("Invalid Email or Username");
            }
            UserModel user = await _db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == userLogin.Email.ToLower() && x.Username.ToLower() == userLogin.Username.ToLower());
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            // If no login code is provided by the user, send email with login code
            if (string.IsNullOrEmpty(userLogin.LoginCode))
            {
                user.LoginCode = Guid.NewGuid().ToString();
                user.LoginCodeExp = DateTime.UtcNow.AddDays(7);
                await _db.SaveChangesAsync();
                await SendEmailCode(user);
                return Ok($"An Email has been sent to {userLogin.Email}");
            }
            // Wrong Token
            if (userLogin.LoginCode != user.LoginCode)
            {
                return BadRequest("Invalid Token");
            }
            // Expired Login Token
            if (userLogin.LoginCode == user.LoginCode && user.LoginCodeExp <= DateTime.UtcNow)
            {
                return BadRequest("Expired Token");
            }
            // Generate & Return JWT
            if (userLogin.LoginCode == user.LoginCode && user.LoginCodeExp >= DateTime.UtcNow)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return BadRequest();
        }

        // Generate a JWT for the provided user object
        private string Generate(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SendEmailCode(UserModel currentUser)
        {
            var email = currentUser.Email;
            var subject = "Fantasy Web App - Email Verification Code";
            var message = $"5 Minute Login Code: {currentUser.LoginCode}";
            var sendEmail = await _emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
