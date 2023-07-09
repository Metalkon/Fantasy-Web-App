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

        // Allowed time to login/register before code/url expires
        private int loginTime = 5;
        private int registerTime = 15;

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
            UserModel user = await _db.Users.SingleAsync(x => x.Email.ToLower() == userLogin.Email.ToLower());
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            // If no login code is provided by the user, send email with login code
            if (string.IsNullOrEmpty(userLogin.LoginCode))
            {
                user.LoginCode = Guid.NewGuid().ToString();
                user.LoginCodeExp = DateTime.UtcNow.AddMinutes(loginTime);
                await _db.SaveChangesAsync();
                await SendEmailCode(user);
                return Ok($"An Email has been sent to {userLogin.Email}");
            }
            // Login Code Checks & Jwt Generation
            if (userLogin.LoginCode != user.LoginCode)
            {
                return BadRequest("Invalid Login Code");
            }
            if (userLogin.LoginCode == user.LoginCode && user.LoginCodeExp <= DateTime.UtcNow)
            {
                return BadRequest("Expired Login Code");
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
            UserModel user = await _db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == userRegister.Email.ToLower());

            // Generate/Update database entry if the user doesn't exist or is unconfirmed
            if (user == null || user.AccountStatus == "Unconfirmed") 
            { 
                if (user != null && user.LoginCodeExp <= DateTime.UtcNow) 
                {
                    return BadRequest($"You are unable to attempt to register again right now, please try again in {registerTime} minutes");
                }
                if (user == null)
                {
                    user = new UserModel()
                    {
                        Email = userRegister.Email,
                        Username = userRegister.Username,
                        Role = "None",
                        LoginCode = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        LoginCodeExp = DateTime.UtcNow.AddMinutes(registerTime)
                    };
                    _db.Users.Add(user);
                }
                else
                {
                    user.Username = userRegister.Username;
                    user.Role = "None";
                    user.LoginCode = Guid.NewGuid().ToString();
                    user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.LoginCodeExp = DateTime.UtcNow.AddMinutes(registerTime);

                }
                await _db.SaveChangesAsync();
                await SendEmailRegister(user);
                return Ok($"A confirmation email has been sent to {user.Email}");
            }
            if (userRegister.Email == user.Email || userRegister.Username == user.Username)
            {
                return BadRequest("Email or Username Has Already Been Taken");
            }
            return BadRequest();
        }

        // Complete user registration
        [AllowAnonymous]
        [HttpPost("confirmation")]
        public async Task<ActionResult<string>> Confirmation(UserConfirm userConfirm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            UserModel user = await _db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == userConfirm.UserRegister.Email.ToLower());
            if (user.LoginCodeExp <= DateTime.UtcNow)
            {
                return BadRequest("The time to confirm your email has expired, please try again");
            }
            // If user information matches with the database, validate account and login the user.
            if (user.Email == userConfirm.UserRegister.Email && user.Username == userConfirm.UserRegister.Username && user.LoginCode == userConfirm.Code)
            {
                user.Role = "User";
                user.AccountStatus = "Validated";
                user.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                var token = Generate(user);
                return Ok(token);
            }
            else 
            {
                return BadRequest("Invalid Confirmation Data");
            }
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
            var subject = "Fantasy Web App - Login Verification Code";
            var message = $"5 Minute Login Code: \n{currentUser.LoginCode}";
            var sendEmail = await _emailSender.SendEmailAsync(currentUser.Email, subject, message);
        }
        private async Task SendEmailRegister(UserModel currentUser)
        {
            var subject = "Fantasy Web App - Comfirm Registration";
            var message = $"5 Minute Registration URL: \n" +
$"https://localhost:7001/confirmation?id={currentUser.Id}&username={currentUser.Username}&email={currentUser.Email}&code={currentUser.LoginCode}";
            var sendEmail = await _emailSender.SendEmailAsync(currentUser.Email, subject, message);
        }
    }
}
