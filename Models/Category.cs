using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";
    }
}