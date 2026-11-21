using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Total price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        public string UserId { get; set; } = "";
    }
}