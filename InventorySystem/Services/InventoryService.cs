using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Services
{
    public class InventoryException : Exception
    {
        public InventoryException(string message) : base(message)
        {
        }
    }

    public interface IInventoryService
    {
        Task ReceiveSupplyAsync(int supplyId);
        Task PlaceOrderAsync(int productId, int quantity, string userId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly InventoryDbContext _db;

        public InventoryService(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task ReceiveSupplyAsync(int supplyId)
        {
            var supply = await _db.Supplies
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == supplyId);

            if (supply == null)
                throw new InventoryException("Supply not found.");

            if (supply.Status == "Received")
                return;

            supply.Product!.Quantity += supply.Quantity;
            supply.Status = "Received";

            await _db.SaveChangesAsync();
        }

        public async Task PlaceOrderAsync(int productId, int quantity, string userId)
        {
            if (quantity <= 0)
                throw new InventoryException("Quantity must be greater than 0.");

            var product = await _db.Products.FindAsync(productId);
            if (product == null)
                throw new InventoryException("Product not found.");

            if (product.Quantity < quantity)
                throw new InventoryException(
                    $"Not enough stock. Available: {product.Quantity}, requested: {quantity}.");

            product.Quantity -= quantity;

            _db.Orders.Add(new Order
            {
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = product.Price * quantity,
                UserId = userId,
                Date = DateTime.Now
            });

            if (product.Quantity < product.MinQuantity)
            {
                bool alreadyOrdered = await _db.Supplies.AnyAsync(
                    s => s.ProductId == productId && s.Status == "Pending");

                var supplier = await _db.Suppliers.FirstOrDefaultAsync();

                if (!alreadyOrdered && supplier != null)
                {
                    _db.Supplies.Add(new Supply
                    {
                        ProductId = productId,
                        SupplierId = supplier.Id,
                        Quantity = product.MinQuantity * 2,
                        Status = "Pending",
                        Date = DateTime.Now
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}