using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = "";

        [Required]
        [Display(Name = "SKU (product code)")]
        public string Sku { get; set; } = "";

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "In stock")]
        public int Quantity { get; set; }

        [Display(Name = "Minimum stock")]
        public int MinQuantity { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}