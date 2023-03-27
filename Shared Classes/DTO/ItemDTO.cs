using System.ComponentModel.DataAnnotations;

namespace Fantasy_Web_API.Models
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
