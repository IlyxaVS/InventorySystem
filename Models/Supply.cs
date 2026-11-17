namespace InventorySystem.Models
{
    public class Supply
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";   // Pending або Received
    }
}