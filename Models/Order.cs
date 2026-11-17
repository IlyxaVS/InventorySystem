namespace InventorySystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string UserId { get; set; } = "";
    }
}