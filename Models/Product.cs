using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Sku { get; set; } = "";     // артикул

        public decimal Price { get; set; }
        public int Quantity { get; set; }          // залишок на складі
        public int MinQuantity { get; set; }       // нижче цього числа робимо автозамовлення

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}