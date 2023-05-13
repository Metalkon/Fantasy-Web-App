using System.ComponentModel.DataAnnotations;

namespace Shared_Classes.Models
{
    public class UserLogin
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string LoginCode { get; set; }
    }
}
