using System.ComponentModel.DataAnnotations;

namespace Shared_Classes.Models
{
    public class UserLogin
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
