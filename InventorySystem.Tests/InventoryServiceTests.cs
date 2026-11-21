using InventorySystem.Data;
using InventorySystem.Models;
using InventorySystem.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Tests
{
    public class InventoryServiceTests
    {
        private static InventoryDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new InventoryDbContext(options);
        }

        private static async Task<Product> AddProductAsync(
            InventoryDbContext db, int quantity, int minQuantity = 0, decimal price = 10)
        {
            var product = new Product
            {
                Name = "Test product",
                Sku = "T-001",
                Price = price,
                Quantity = quantity,
                MinQuantity = minQuantity
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return product;
        }

        private static async Task<Supplier> AddSupplierAsync(InventoryDbContext db)
        {
            var supplier = new Supplier { Name = "Test supplier" };
            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();
            return supplier;
        }

        private static async Task<Supply> AddSupplyAsync(
            InventoryDbContext db, Product product, Supplier supplier, int quantity, string status = "Pending")
        {
            var supply = new Supply
            {
                ProductId = product.Id,
                SupplierId = supplier.Id,
                Quantity = quantity,
                Status = status
            };
            db.Supplies.Add(supply);
            await db.SaveChangesAsync();
            return supply;
        }

        [Fact]
        public async Task PlaceOrder_ReducesStock()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 10);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            Assert.Equal(7, (await db.Products.FindAsync(product.Id))!.Quantity);
        }

        [Fact]
        public async Task PlaceOrder_SavesOrderWithCorrectTotal()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 10, price: 2.5m);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            var order = await db.Orders.SingleAsync();
            Assert.Equal(3, order.Quantity);
            Assert.Equal(7.5m, order.TotalPrice);
        }

        [Fact]
        public async Task PlaceOrder_SavesUserId()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 10);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 1, "user-42");

            var order = await db.Orders.SingleAsync();
            Assert.Equal("user-42", order.UserId);
        }

        [Fact]
        public async Task PlaceOrder_NotEnoughStock_ThrowsAndChangesNothing()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 2);
            var service = new InventoryService(db);

            await Assert.ThrowsAsync<InventoryException>(
                () => service.PlaceOrderAsync(product.Id, 5, "user1"));

            Assert.Equal(2, (await db.Products.FindAsync(product.Id))!.Quantity);
            Assert.Empty(db.Orders);
        }

        [Fact]
        public async Task PlaceOrder_ExactStock_IsAllowedAndStockBecomesZero()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 5);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 5, "user1");

            Assert.Equal(0, (await db.Products.FindAsync(product.Id))!.Quantity);
            Assert.Single(db.Orders);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task PlaceOrder_InvalidQuantity_Throws(int quantity)
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 10);
            var service = new InventoryService(db);

            await Assert.ThrowsAsync<InventoryException>(
                () => service.PlaceOrderAsync(product.Id, quantity, "user1"));

            Assert.Equal(10, (await db.Products.FindAsync(product.Id))!.Quantity);
        }

        [Fact]
        public async Task PlaceOrder_UnknownProduct_Throws()
        {
            var db = CreateDb();
            var service = new InventoryService(db);

            await Assert.ThrowsAsync<InventoryException>(
                () => service.PlaceOrderAsync(999, 1, "user1"));
        }

        [Fact]
        public async Task PlaceOrder_LowStock_CreatesPendingSupply()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 6, minQuantity: 5);
            await AddSupplierAsync(db);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            var supply = await db.Supplies.SingleAsync();
            Assert.Equal("Pending", supply.Status);
            Assert.Equal(product.Id, supply.ProductId);
            Assert.Equal(10, supply.Quantity);
        }

        [Fact]
        public async Task PlaceOrder_StockStaysAboveMinimum_CreatesNoSupply()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 20, minQuantity: 5);
            await AddSupplierAsync(db);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            Assert.Empty(db.Supplies);
        }

        [Fact]
        public async Task PlaceOrder_PendingSupplyExists_DoesNotCreateSecondOne()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 6, minQuantity: 5);
            var supplier = await AddSupplierAsync(db);
            await AddSupplyAsync(db, product, supplier, 10);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            Assert.Single(db.Supplies);
        }

        [Fact]
        public async Task PlaceOrder_LowStockButNoSupplier_StillSavesOrder()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 6, minQuantity: 5);
            var service = new InventoryService(db);

            await service.PlaceOrderAsync(product.Id, 3, "user1");

            Assert.Single(db.Orders);
            Assert.Empty(db.Supplies);
        }

        [Fact]
        public async Task ReceiveSupply_IncreasesStockAndMarksReceived()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 5);
            var supplier = await AddSupplierAsync(db);
            var supply = await AddSupplyAsync(db, product, supplier, 20);
            var service = new InventoryService(db);

            await service.ReceiveSupplyAsync(supply.Id);

            Assert.Equal(25, (await db.Products.FindAsync(product.Id))!.Quantity);
            Assert.Equal("Received", (await db.Supplies.FindAsync(supply.Id))!.Status);
        }

        [Fact]
        public async Task ReceiveSupply_CalledTwice_AddsStockOnlyOnce()
        {
            var db = CreateDb();
            var product = await AddProductAsync(db, quantity: 5);
            var supplier = await AddSupplierAsync(db);
            var supply = await AddSupplyAsync(db, product, supplier, 20);
            var service = new InventoryService(db);

            await service.ReceiveSupplyAsync(supply.Id);
            await service.ReceiveSupplyAsync(supply.Id);

            Assert.Equal(25, (await db.Products.FindAsync(product.Id))!.Quantity);
        }

        [Fact]
        public async Task ReceiveSupply_UnknownSupply_Throws()
        {
            var db = CreateDb();
            var service = new InventoryService(db);

            await Assert.ThrowsAsync<InventoryException>(
                () => service.ReceiveSupplyAsync(999));
        }
    }
}