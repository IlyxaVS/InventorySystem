using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace InventorySystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly InventoryDbContext _context;

        public ReportsController(InventoryDbContext context)
        {
            _context = context;
        }

        private async Task<List<SalesRow>> GetSalesAsync(DateTime from, DateTime to)
        {
            var end = to.Date.AddDays(1);

            return await _context.Orders
                .Where(o => o.Date >= from.Date && o.Date < end)
                .GroupBy(o => o.Product!.Name)
                .Select(g => new SalesRow
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(o => o.Quantity),
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var fromDate = from ?? DateTime.Today.AddDays(-30);
            var toDate = to ?? DateTime.Today;

            ViewBag.From = fromDate.ToString("yyyy-MM-dd");
            ViewBag.To = toDate.ToString("yyyy-MM-dd");

            ViewBag.Stock = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var sales = await GetSalesAsync(fromDate, toDate);
            return View(sales);
        }

        public async Task<IActionResult> StockCsv()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Name,SKU,Category,Warehouse,Price,InStock,MinimumStock,Value");
            foreach (var p in products)
            {
                sb.AppendLine(string.Join(",",
                    Csv(p.Name), Csv(p.Sku), Csv(p.Category?.Name), Csv(p.Warehouse?.Name),
                    p.Price.ToString(CultureInfo.InvariantCulture),
                    p.Quantity, p.MinQuantity,
                    (p.Price * p.Quantity).ToString(CultureInfo.InvariantCulture)));
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "stock-report.csv");
        }

        public async Task<IActionResult> SalesCsv(DateTime from, DateTime to)
        {
            var sales = await GetSalesAsync(from, to);

            var sb = new StringBuilder();
            sb.AppendLine("Product,QuantitySold,Revenue");
            foreach (var s in sales)
            {
                sb.AppendLine(string.Join(",",
                    Csv(s.ProductName), s.Quantity,
                    s.Revenue.ToString(CultureInfo.InvariantCulture)));
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "sales-report.csv");
        }

        private static string Csv(string? value)
        {
            return "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
        }
    }
}