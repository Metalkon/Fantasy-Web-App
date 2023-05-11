using Fantasy_Web_API.Models;
using Fantasy_Web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared_Classes.Models;
using System.Data;
using System.Security.Claims;

namespace Fantasy_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        public UserController(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult> TestEmail(string email, string subject, string message)
        {
            await _emailSender.SendEmailAsync(email, subject, message);

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult IdentifyUser()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Greetings {currentUser.Username}, your role is \"{currentUser.Role}\", and your email is \"{currentUser.Email}\".");
        }

        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserModel
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}
