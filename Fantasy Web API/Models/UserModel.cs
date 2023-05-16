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
        public string Role { get; set; }
        [Required]
        public string AccountStatus { get; set; }
        [Required]
        public string LoginStatus { get; set; }
        [Required]
        public string LoginCode { get; set; }
        [Required]
        public DateTime LoginCodeExp { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //public UserGameData GameData { get; set; }
    }
}
