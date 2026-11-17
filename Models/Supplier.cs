using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Phone { get; set; }
    }
}