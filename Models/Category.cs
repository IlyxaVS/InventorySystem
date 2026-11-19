using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = "";
    }
}