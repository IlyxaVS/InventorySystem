using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть назву")]
        [Display(Name = "Назва")]
        public string Name { get; set; } = "";

        [Display(Name = "Адреса")]
        public string? Address { get; set; }
    }
}