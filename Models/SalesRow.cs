namespace InventorySystem.Models
{
    public class SalesRow
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }
}