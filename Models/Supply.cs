using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Supply
    {
        public int Id { get; set; }

        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";   // Pending or Received
    }
}