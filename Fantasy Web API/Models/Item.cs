using System.ComponentModel.DataAnnotations;

namespace Fantasy_Web_API.Models
{
    public class Item
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public int Price { get; set; } = 1;
        public string Description { get; set; } = "N/A";
        public string? Image { get; set; }
    }
}
