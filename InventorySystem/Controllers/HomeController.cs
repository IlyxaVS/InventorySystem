using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InventorySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly InventoryDbContext _context;

        public HomeController(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                ViewBag.ProductsCount = await _context.Products.CountAsync();
                ViewBag.LowStockCount = await _context.Products.CountAsync(p => p.Quantity < p.MinQuantity);
                ViewBag.OrdersCount = await _context.Orders.CountAsync();
                ViewBag.PendingSupplies = await _context.Supplies.CountAsync(s => s.Status == "Pending");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}