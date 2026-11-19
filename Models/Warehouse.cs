using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = "";

        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}