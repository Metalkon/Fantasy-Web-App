using Fantasy_Web_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Shared_Classes.Models;
using System.Text;
using Fantasy_Web_API.Models;
using Fantasy_Web_API.Services;
using System.Text.RegularExpressions;

namespace Fantasy_Web_API.Controllers
{
    [Route("user")]
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
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid || !Regex.IsMatch(userLogin.Username, @"^[a-zA-Z0-9\s]+$"))
            {
                return BadRequest("Invalid Input");
            }
            if (string.IsNullOrEmpty(userLogin.Username) || string.IsNullOrEmpty(userLogin.Email))
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
            // Login Code Checks & Jwt Generation
            if (userLogin.LoginCode != user.LoginCode)
            {
                return BadRequest("Invalid Token");
            }
            if (userLogin.LoginCode == user.LoginCode && user.LoginCodeExp <= DateTime.UtcNow)
            {
                return BadRequest("Expired Token");
            }
            if (userLogin.LoginCode == user.LoginCode && user.LoginCodeExp >= DateTime.UtcNow)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return BadRequest();
        }

        // Handle user registration and send confirmation email
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegister userRegister)
        {
            if (!ModelState.IsValid || userRegister == null || string.IsNullOrEmpty(userRegister.Username) || string.IsNullOrEmpty(userRegister.Email))
            {
                return BadRequest("Invalid Email or Username");
            }

            bool checkUser = await _db.Users.AnyAsync(x => x.Email.ToLower() == userRegister.Email.ToLower() || x.UpdatedAt == DateTime.UtcNow.AddMinutes(-10));
            if (checkUser == true)
            {
                return BadRequest("Email Has Already Been Taken");
            }
            bool checkUsername = await _db.Users.AnyAsync(x => x.Username.ToLower() == userRegister.Username.ToLower());
            if (checkUsername == true)
            {
                return BadRequest("Username Has Already Been Taken");
            }
            return Ok("no issues... yet");
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
