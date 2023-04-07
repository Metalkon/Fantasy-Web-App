using System.ComponentModel.DataAnnotations;

namespace Shared_Classes.Models
{
    public class ItemSearchResponse<T>
    {
        public int PageId { get; set; }
        public IEnumerable<ItemDTO> Data { get; set; }
    }
}
