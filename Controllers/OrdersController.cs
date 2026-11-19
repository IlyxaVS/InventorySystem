using InventorySystem.Data;
using InventorySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryService _inventory;

    public OrdersController(ApplicationDbContext context, IInventoryService inventory)
    {
        _context = context;
        _inventory = inventory;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Product)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return View(orders);
    }


    public IActionResult Create()
    {
        ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int productId, int quantity)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            await _inventory.PlaceOrderAsync(productId, quantity, userId);
            return RedirectToAction(nameof(Index));
        }
        catch (InventoryException ex)
        {
            ViewBag.Error = ex.Message;
        }

        ViewBag.Products = new SelectList(_context.Products, "Id", "Name", productId);
        return View();
    }

    private bool OrderExists(int? id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
