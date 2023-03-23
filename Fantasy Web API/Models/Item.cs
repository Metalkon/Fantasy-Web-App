using System.ComponentModel.DataAnnotations;

namespace Fantasy_Web_API.Models
{
    public class Item
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
