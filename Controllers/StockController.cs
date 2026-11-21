using InventorySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly InventoryDbContext _context;

        public StockController(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }
    }
}