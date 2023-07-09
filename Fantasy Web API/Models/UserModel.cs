using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Fantasy_Web_API.Models
{
    public class UserModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; } = "None";
        [Required]
        public string AccountStatus { get; set; } = string.Empty;
        [Required]
        public string LoginStatus { get; set; } = string.Empty;
        [Required]
        public string LoginCode { get; set; } = string.Empty;
        [Required]
        public DateTime LoginCodeExp { get; set; } = DateTime.Now;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //public UserGameData GameData { get; set; }
    }
}
